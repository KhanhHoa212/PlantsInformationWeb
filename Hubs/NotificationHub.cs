using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrapeCity.ActiveReports.Expressions.Remote.GlobalDataTypes;
using Microsoft.AspNetCore.SignalR;

namespace PlantsInformationWeb.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendLockNotification(string userId, string reason)
        {
            await Clients.User(userId).SendAsync("AccountLocked", reason);
        }

    }
}