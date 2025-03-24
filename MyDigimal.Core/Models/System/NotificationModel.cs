using System;
using System.Dynamic;
using MyDigimal.Common;

namespace MyDigimal.Core.Models.System
{
    public class NotificationModel
    {
        public Guid Id { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ExpandoObject MetaData { get; set; }
        public Guid Author { get; set; }
        public string AuthorName { get; set; }
        public Guid Recipient { get; set; }
        public DateTime Created { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
    }
}