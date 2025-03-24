using System;
using MyDigimal.Common;
using MyDigimal.Core.Handlers.Processors;

namespace MyDigimal.Core.Handlers
{
    public class NotificationDeclineProcessorFactory : INotificationProcessorFactory<INotificationDeclineProcessor>
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationDeclineProcessorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public INotificationDeclineProcessor GetProcessor(NotificationType type)
        {
            INotificationDeclineProcessor notificationProcessor = null;
            
            switch (type)
            {
                case NotificationType.DigimalAwaitingTransferTo:
                    notificationProcessor = (INotificationDeclineProcessor)_serviceProvider.GetService(typeof(DigimalAwaitingTransferToDeclinedProcessor));
                    break;
            }

            return notificationProcessor;
        }
    }
}