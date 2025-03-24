using System;
using MyDigimal.Common;
using MyDigimal.Core.Handlers.Processors;

namespace MyDigimal.Core.Handlers
{
    public class NotificationAcceptanceProcessorFactory : INotificationProcessorFactory<INotificationAcceptanceProcessor>
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationAcceptanceProcessorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public INotificationAcceptanceProcessor GetProcessor(NotificationType type)
        {
            INotificationAcceptanceProcessor notificationProcessor = null;
            
            switch (type)
            {
                case NotificationType.DigimalAwaitingTransferTo:
                    notificationProcessor = (INotificationAcceptanceProcessor)_serviceProvider.GetService(typeof(DigimalAwaitingTransferToAcceptanceProcessor));
                    break;
            }

            return notificationProcessor;
        }
    }
}