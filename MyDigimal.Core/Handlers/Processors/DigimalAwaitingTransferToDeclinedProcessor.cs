using System;
using System.Threading.Tasks;
using MyDigimal.Common;
using MyDigimal.Data;
using MyDigimal.Core.Models.System;
using MyDigimal.Core.Models.System.NotificatiosnMetaData;
using Newtonsoft.Json;

namespace MyDigimal.Core.Handlers.Processors
{
    public class DigimalAwaitingTransferToDeclinedProcessor : BaseNotificationProcessor, INotificationDeclineProcessor
    {
        public DigimalAwaitingTransferToDeclinedProcessor(IUnitOfWork unitOfWork) : base(unitOfWork) {}

        public async Task ProcessAsync(NotificationModel notificationModel)
        {
            var jsonMetaData = JsonConvert.SerializeObject(notificationModel.MetaData);
            var metaData = JsonConvert.DeserializeObject<DigimalMetaDataModel>(jsonMetaData);
            
            //Transfer digimal to new owner
            var digimal = await UnitOfWork.Creatures.GetByIdAsync(metaData.DigimalId, notificationModel.Recipient);

            if (digimal != null)
            {
                //mark as read
                await MarkAsReadAsync(notificationModel);

                //raise notification with new owner creature has been transferred to them
                var xferToNotification = new NotificationModel
                {
                    Type = NotificationType.DigimalTransferFailed,
                    Title =  "Digimal transferred declined!",
                    Description = "Transfer of new Digimal to your account was declined by the owner.",
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
                    Type = NotificationType.DigimalTransferFailed,
                    Title =  "Digimal transferred declined!",
                    Description = $"You declined the transfer of your Digimal ( {digimal.Name} - #{digimal.CommonName}) to {notificationModel.AuthorName}",
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