// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;
// using Microsoft.Extensions.Logging;
// using PlantsInformationWeb.DTOs;
// using PlantsInformationWeb.Services;

// namespace PlantsInformationWeb.Pages.Shared
// {
//     public class _Chatbox : PageModel
//     {
//         private readonly ILogger<_Chatbox> _logger;
//         private readonly ChatService _chatService;

//         [BindProperty]
//         public string UserInput { get; set; }

//         public int CurrentSessionId { get; set; }
//         public List<ChatMessageDto> ChatHistory { get; set; }
//         public int UserId { get; set; }

//         public _Chatbox(ILogger<_Chatbox> logger, ChatService chatService)
//         {
//             _logger = logger;
//             _chatService = chatService;
//         }

//         public async Task<IActionResult> OnGetAsync(int? sessionId)
//         {

//             if (sessionId == null || sessionId == 0)
//             {
//                 var session = await _chatService.CreateSessionAsync(UserId);
//                 CurrentSessionId = session.SessionId;
//             }
//             else
//             {
//                 CurrentSessionId = sessionId.Value;
//             }
//             ChatHistory = await _chatService.GetChatMessageBySessionAsync(CurrentSessionId);
//             return Page();
//         }

//         public async Task<IActionResult> OnPostSendMessageAsync(int sessionId)
//         {
//             CurrentSessionId = sessionId;

//             await _chatService.SendMessageAsync(sessionId, UserId, UserInput);

//             ChatHistory = await _chatService.GetChatMessageBySessionAsync(sessionId);

//             UserInput = string.Empty;

//             return Page();
//         }
//     }
// }