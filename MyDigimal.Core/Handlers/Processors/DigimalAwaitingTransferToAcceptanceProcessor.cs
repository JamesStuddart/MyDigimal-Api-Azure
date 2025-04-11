using System;
using System.Threading.Tasks;
using MyDigimal.Common;
using MyDigimal.Data;
using MyDigimal.Data.Entities.Creatures;
using MyDigimal.Core.Models.System;
using MyDigimal.Core.Models.System.NotificatiosnMetaData;
using MyDigimal.Core.Serialization;
using Newtonsoft.Json;

namespace MyDigimal.Core.Handlers.Processors
{
    public class DigimalAwaitingTransferToAcceptanceProcessor(IUnitOfWork unitOfWork)
        : BaseNotificationProcessor(unitOfWork), INotificationAcceptanceProcessor
    {
        public async Task ProcessAsync(NotificationModel notificationModel)
        {
            var jsonMetaData = JsonSerialization.Serialize(notificationModel.MetaData);
            var metaData = JsonConvert.DeserializeObject<DigimalMetaDataModel>(jsonMetaData);
            
            //Transfer digimal to new owner
            var creature = await UnitOfWork.Creatures.GetByIdAsync(metaData.DigimalId, notificationModel.Recipient);

            if (creature != null)
            {
                creature.Owner = notificationModel.Author;
                await UnitOfWork.Creatures.UpdateAsync(creature);
                await UnitOfWork.CommitAsync();

                await UnitOfWork.CreatureEvents.InsertAsync(new CreatureEventEntity
                {
                    CreatureId = creature.Id.Value,
                    Event = (int) CreatureEventType.OwnerChange,
                    ValueName = "Owner",
                    OriginalValue = notificationModel.Author.ToString(),
                    NewValue = notificationModel.Recipient.ToString(),
                    EventDate = DateTime.UtcNow,
                    CreatedBy = notificationModel.Author
                }, false);
                await UnitOfWork.CommitAsync();
                
                //mark as read
                await MarkAsReadAsync(notificationModel);
                
                //raise notification with new owner creature has been transferred to them
                var xferToNotification = new NotificationModel
                {
                    Type = NotificationType.DigimalTransferredTo,
                    Title =  "Digimal transferred to you!",
                    Description = $"You have a new Digimal, go check out { creature.Name } now!",
                    MetaData = notificationModel.MetaData,
                    Author = Guid.Empty,
                    AuthorName = "myDigimal",
                    Recipient = notificationModel.Author,
                    Created =  DateTime.UtcNow
                };
                
                await RaiseNotificationAsync(xferToNotification);
                    
                //raise notification with previous owner creature has been transferred to new owner
                var xferToNewOwnerNotification = new NotificationModel
                {
                    Type = NotificationType.DigimalTransferredFrom,
                    Title =  "Digimal transferred to new owner!",
                    Description = $"Your Digimal, { creature.Name } (#{creature.ShortCode}), was successfully transferred to its new owner!",
                    MetaData = notificationModel.MetaData,
                    Author = Guid.Empty,
                    AuthorName = "myDigimal",
                    Recipient = notificationModel.Recipient,
                    Created =  DateTime.UtcNow
                };
                
                await RaiseNotificationAsync(xferToNewOwnerNotification);
                await UnitOfWork.CommitAsync();
            }
        }
    }
}