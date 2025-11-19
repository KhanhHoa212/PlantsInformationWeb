using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Repository;
 
namespace PlantsInformationWeb.Services
{
    public class PlantCommentService
    {
        private readonly IPlantCommentRepository _plantCommentRepository;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Plantcomment> _plantCommentRepo;
        private readonly NotificationService _notificationService;
 
        public PlantCommentService(IMapper mapper, NotificationService notificationService, IPlantCommentRepository plantCommentRepository, IGenericRepository<Plantcomment> plantCommentRepo)
        {
            _plantCommentRepository = plantCommentRepository;
            _plantCommentRepo = plantCommentRepo;
            _mapper = mapper;
            _notificationService = notificationService;
        }
        public async Task<List<PlantCommentDto>> GetCommentsAsync(int plantId)
        {
            return await _plantCommentRepository.GetCommentsByPlantId(plantId);
        }
 
        public async Task<PlantCommentDto> AddCommentAsync(PlantCommentDto plantComment)
        {
            var comment = new Plantcomment
            {
                PlantId = plantComment.PlantId,
                UserId = plantComment.UserId,
                ParentCommentId = plantComment.ParentCommentId,
                CommentText = plantComment.CommentText,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsDeleted = false
            };
 
            await _plantCommentRepo.AddAsync(comment);
 
            if (comment.ParentCommentId != null)
            {
                var parentComment = await _plantCommentRepo.GetByIdAsync(comment.ParentCommentId.Value);
                // Kiểm tra: Không gửi cho chính mình
                if (parentComment != null && parentComment.UserId != comment.UserId)
                {
                    var notiDto = new NotificationDto
                    {
                        UserId = parentComment.UserId,
                        PlantId = comment.PlantId,
                        CommentId = comment.CommentId,
                        Message = $"{comment.User.Username} đã trả lời bình luận của bạn.",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };
 
                    await _notificationService.AddReplyCommentNotificationAsync(notiDto);
                }
            }
 
            // Lấy comment vừa thêm, kèm User
            var savedComment = await _plantCommentRepository.GetCommentWithUserByIdAsync(comment.CommentId);
 
            // Map sang DTO có UserName
            return _mapper.Map<PlantCommentDto>(savedComment);
        }
    }
}