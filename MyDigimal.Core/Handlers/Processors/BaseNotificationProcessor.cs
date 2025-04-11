using System;
using System.Dynamic;
using System.Threading.Tasks;
using MyDigimal.Common;
using MyDigimal.Core.Models.System;
using MyDigimal.Core.Serialization;
using MyDigimal.Data;
using MyDigimal.Data.Entities.System;
using Newtonsoft.Json;

namespace MyDigimal.Core.Handlers.Processors
{
    public abstract class BaseNotificationProcessor
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected BaseNotificationProcessor(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
        
        internal async Task MarkAsReadAsync(NotificationModel notificationModel)
        {
            notificationModel.DateRead = DateTime.UtcNow;
            await UnitOfWork.Notifications.UpdateAsync(MapToDataModel(notificationModel));
        }

        internal async Task RaiseNotificationAsync( NotificationModel notificationModel)
        {
            await UnitOfWork.Notifications.InsertAsync(MapToDataModel(notificationModel));
        }
        
        private NotificationModel MapToViewModel(MyDigimal.Data.Entities.System.NotificationEntity notificationEntity)
            => new  NotificationModel
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
        
        private MyDigimal.Data.Entities.System.NotificationEntity MapToDataModel(NotificationModel notificationModel)
            => new MyDigimal.Data.Entities.System.NotificationEntity
            {
                Id = notificationModel.Id,
                Type = (int) notificationModel.Type,
                Title = notificationModel.Title,
                Description = notificationModel.Description,
                MetaData = JsonSerialization.Serialize(notificationModel.MetaData),
                Author = notificationModel.Author,
                AuthorName = notificationModel.AuthorName,
                Recipient = notificationModel.Recipient,
                Created = notificationModel.Created,
                DateRead = notificationModel.DateRead,
            };
        
    }
}