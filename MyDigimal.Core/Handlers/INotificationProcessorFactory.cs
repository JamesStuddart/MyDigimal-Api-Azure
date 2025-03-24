using MyDigimal.Common;

namespace MyDigimal.Core.Handlers
{
    public interface INotificationProcessorFactory<T> where T : class
    {
        T GetProcessor(NotificationType type);
    }
}