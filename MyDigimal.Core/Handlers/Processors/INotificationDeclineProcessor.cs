using System.Threading.Tasks;
using MyDigimal.Core.Models.System;

namespace MyDigimal.Core.Handlers.Processors
{
    public interface INotificationDeclineProcessor
    {
        Task ProcessAsync(NotificationModel notificationModel);

    }
}