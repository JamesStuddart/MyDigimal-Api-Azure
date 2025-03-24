using System;

namespace MyDigimal.Data.Entities.System
{
    public class NotificationEntity
    {
        public Guid Id { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string MetaData { get; set; }
        public Guid Author { get; set; }
        public string AuthorName { get; set; }
        public Guid Recipient { get; set; }
        public DateTime Created { get; set; }
        public DateTime? DateRead { get; set; }
    }
}