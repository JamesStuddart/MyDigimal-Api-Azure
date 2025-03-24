using System.Threading.Tasks;
using MyDigimal.Core.Models.System;

namespace MyDigimal.Core.Handlers.Processors
{
    public interface INotificationAcceptanceProcessor
    {
        Task ProcessAsync(NotificationModel notificationModel);
    }
}