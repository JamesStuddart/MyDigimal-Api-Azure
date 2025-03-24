using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using MyDigimal.Common;
using MyDigimal.Data;
using MyDigimal.Core.Handlers.Processors;
using MyDigimal.Core.Models.System;
using Newtonsoft.Json;

namespace MyDigimal.Core.Handlers
{
    public class NotificationHandler(
        IUnitOfWork unitOfWork,
        INotificationProcessorFactory<INotificationAcceptanceProcessor> notificationAcceptanceProcessor,
        INotificationProcessorFactory<INotificationDeclineProcessor> notificationDeclineProcessor)
        : INotificationHandler
    {
        public async Task RaiseNotificationAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task MarkAsReadASync(Guid id, Guid userId)
        {
            var notification = await unitOfWork.Notifications.GetByIdAsync(id);

            if (notification != null && notification.Recipient.Equals(userId))
            {
                notification.DateRead = DateTime.UtcNow;
                await unitOfWork.Notifications.UpdateAsync(notification);
                await unitOfWork.CommitAsync();
            }
        }

        public async Task AcceptNotificationAsync(Guid id, Guid userId)
        {
            var notification = await unitOfWork.Notifications.GetByIdAsync(id);
            var processor = notificationAcceptanceProcessor.GetProcessor((NotificationType) notification.Type);

            if (processor != null)
            {
                await processor.ProcessAsync(MapToNotification(notification));
            }
        }

        public async Task DeclineNotificationAsync(Guid id, Guid userId)
        {
            var notification = await unitOfWork.Notifications.GetByIdAsync(id);
            var processor = notificationDeclineProcessor.GetProcessor((NotificationType) notification.Type);

            if (processor != null)
            {
                await processor.ProcessAsync(MapToNotification(notification));
            }
        }

        public async Task<IEnumerable<Models.System.NotificationModel>> GetNotificationsByUserIdAsync(Guid userId)
        {
            var notifications = await unitOfWork.Notifications.GetByUserId(userId);
            await unitOfWork.AbortAsync();

            return notifications.Select(MapToNotification);
        }

        private NotificationModel MapToNotification(MyDigimal.Data.Entities.System.NotificationEntity notificationEntity)
            => new NotificationModel
            {
                Id = notificationEntity.Id,
                Type = (NotificationType) notificationEntity.Type,
                Title = notificationEntity.Title,
                Description = notificationEntity.Description,
                MetaData = JsonConvert.DeserializeObject<ExpandoObject>(notificationEntity.MetaData),
                Author = notificationEntity.Author,
                AuthorName = notificationEntity.AuthorName,
                Recipient = notificationEntity.Recipient,
                Created = notificationEntity.Created,
                IsRead = notificationEntity.DateRead.HasValue,
                DateRead = notificationEntity.DateRead,
            };
    }
}