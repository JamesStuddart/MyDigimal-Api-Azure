using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyDigimal.Core.Models.System;

namespace MyDigimal.Core.Handlers
{
    public interface INotificationHandler
    {
        Task RaiseNotificationAsync(Guid userId);
        Task MarkAsReadASync(Guid id, Guid userId);
        Task AcceptNotificationAsync(Guid id, Guid userId);
        Task DeclineNotificationAsync(Guid id, Guid userId);
        Task<IEnumerable<NotificationModel>> GetNotificationsByUserIdAsync(Guid userId);
    }
}