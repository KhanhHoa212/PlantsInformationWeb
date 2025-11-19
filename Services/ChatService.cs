using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Repository;

namespace PlantsInformationWeb.Services
{
    public class ChatService
    {
        private readonly IGenericRepository<Chatmessage> _chatMessageRepo;
        private readonly IGenericRepository<Chatsession> _chatSessionRepo;
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<Unrecognizedplant> _unPlant;

        private readonly IMapper _mapper;
        private readonly AIService _aiService;


        public ChatService(IGenericRepository<User> userRepo, IGenericRepository<Unrecognizedplant> unPlant, AIService aIService, IMapper mapper, IGenericRepository<Chatmessage> messageRepo, IGenericRepository<Chatsession> sessionRepo)
        {
            _chatMessageRepo = messageRepo;
            _chatSessionRepo = sessionRepo;
            _mapper = mapper;
            _userRepo = userRepo;
            _aiService = aIService;
            _unPlant = unPlant;

        }

        public async Task<ChatMessageDto> AddMessageAsync(ChatMessageDto message)
        {
            var entity = _mapper.Map<Chatmessage>(message);

            await _chatMessageRepo.AddAsync(entity);

            var resultDto = _mapper.Map<ChatMessageDto>(entity);
            return resultDto;
        }

        public async Task<ChatSessionDto> CreateSessionAsync(int userId)
        {
            Console.WriteLine($"[ChatService] Tạo session cho userId={userId}");
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
            {
                Console.WriteLine("[ChatService] Không tìm thấy user");
                throw new Exception("UserId không tồn tại. Không thể tạo session!");
            }

            var session = new Chatsession
            {
                UserId = userId,
                StartedAt = DateTime.Now
            };
            Console.WriteLine("[ChatService] Trước khi AddAsync vào repo");
            await _chatSessionRepo.AddAsync(session);
            Console.WriteLine($"[ChatService] Sau khi AddAsync, SessionId={session.SessionId}");
            var resultDto = _mapper.Map<ChatSessionDto>(session);
            return resultDto;
        }

        public async Task<List<ChatMessageDto>> GetChatMessageBySessionAsync(int sessionId)
        {
            var allMessages = await _chatMessageRepo.GetAllAsync();
            var sessionMessage = allMessages
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.SentAt)
                .ToList();
            var dtos = _mapper.Map<List<ChatMessageDto>>(sessionMessage);
            return dtos;
        }

        public async Task EndSessionAsync(int sessionId)
        {
            var allSessions = await _chatSessionRepo.GetAllAsync();
            var session = await _chatSessionRepo.GetByIdAsync(sessionId);
            if (session != null)
            {
                session.EndedAt = DateTime.Now;
                await _chatSessionRepo.UpdateAsync(session);
            }
        }

        private async Task<string> CallAIAsync(List<MessageDto> messages, string userMessage)
        {

            return await _aiService.AskAIAsync(messages, userMessage);

        }

        public async Task<ChatMessageDto> SendMessageAsync(int sessionId, int userId, string userMessage)
        {
            // 1. Thêm tin nhắn user vào DB như cũ
            var userMsg = new ChatMessageDto
            {
                SessionId = sessionId,
                UserId = userId,
                SenderType = "user",
                MessageText = userMessage,
                SentAt = DateTime.Now
            };
            await AddMessageAsync(userMsg);

            var unknownPlants = await _aiService.CheckIsExitedPlantAsync(userMessage);

            if (unknownPlants != null && unknownPlants.Count > 0)
            {
                var loggedPlants = await _unPlant.GetAllAsync();

                foreach (var name in unknownPlants)
                {
                    if (!loggedPlants.Any(up => up.Plantname.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    {
                        var entry = new Unrecognizedplant
                        {
                            Plantname = name,
                            Usermessage = userMessage,
                            Createdat = DateTime.Now
                        };
                        await _unPlant.AddAsync(entry);
                    }
                }
            }

            // 2. Lấy lịch sử chat của session
            var history = await GetChatMessageBySessionAsync(sessionId);
            var lastFiveMessages = history
                .OrderByDescending(m => m.SentAt)
                .Take(8)
                .OrderBy(m => m.SentAt)
                .ToList();

            // 3. Chuyển lịch sử sang mảng messages cho AI
            var messages = new List<MessageDto>();
            foreach (var msg in lastFiveMessages)
            {
                messages.Add(new MessageDto
                {
                    role = msg.SenderType == "user" ? "user" : "assistant",
                    content = msg.MessageText
                });
            }

            // Gọi AI với toàn bộ lịch sử
            var aiReply = await _aiService.AskAIAsync(messages, userMessage);

            // 4. Lưu trả lời AI vào DB như cũ
            var aiMsg = new ChatMessageDto
            {
                SessionId = sessionId,
                UserId = userId,
                SenderType = "ai",
                MessageText = aiReply,
                SentAt = DateTime.Now
            };
            await AddMessageAsync(aiMsg);

            return aiMsg;
        }

    }
}