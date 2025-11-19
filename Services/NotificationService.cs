using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Hubs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Repository;

namespace PlantsInformationWeb.Services
{
    public class NotificationService
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Notification> _notification;
        private readonly IHubContext<NotificationHub> _notificationHubContext;

        public NotificationService(IGenericRepository<Notification> notification, IMapper mapper, IHubContext<NotificationHub> notificationHubContext)
        {
            _notification = notification;
            _mapper = mapper;
            _notificationHubContext = notificationHubContext;
        }

        public async Task<NotificationDto> AddReplyCommentNotificationAsync(NotificationDto notification)
        {
            var entity = _mapper.Map<Notification>(notification);

            await _notification.AddAsync(entity);

            // Push notification to receiver via SignalR
            await _notificationHubContext.Clients.User(notification.UserId.ToString())
                .SendAsync("NewNotification", new
                {
                    title = "Thông báo mới",
                    message = notification.Message,
                    timestamp = notification.CreatedAt?.ToString("HH:mm dd/MM/yyyy")
                });

            var resultDto = _mapper.Map<NotificationDto>(entity);
            return resultDto;
        }

        public async Task<List<NotificationDto>> GetNotificationA(int userId)
        {
            var noti = await _notification.GetAllAsync();
            var listNoti = noti.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAt).ToList();
            var dtos = _mapper.Map<List<NotificationDto>>(listNoti);
            return dtos;
        }

        public async Task MarkNotificationAsReadAsync(int userId, int notiId)
        {
            var noti = await _notification.GetByIdAsync(notiId);
            if (noti != null && noti.UserId == userId)
            {
                noti.IsRead = true;
                await _notification.UpdateAsync(noti);
            }

        }
    }
}