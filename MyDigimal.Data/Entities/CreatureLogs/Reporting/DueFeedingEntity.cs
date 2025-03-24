using System;
using MyDigimal.Common;

namespace MyDigimal.Data.Entities.CreatureLogs.Reporting
{
    public class DueFeedingEntity
    {
        public Guid CreatureId { get; set; }
        public Guid SchemaEntryId { get; set; }
        public string Name { get; set; }
        public DateTime LastFeedDate { get; set; }
        public int FeedingCadence { get; set; }
        public FeedingCadenceType FeedingCadenceType { get; set; }

        public DateTime? NextFeedDate =>
            FeedingCadenceType switch
            {
                FeedingCadenceType.NotSet => null,
                FeedingCadenceType.Hours => LastFeedDate.AddHours(FeedingCadence),
                FeedingCadenceType.Days => LastFeedDate.AddDays(FeedingCadence),
                FeedingCadenceType.Weeks => LastFeedDate.AddDays(FeedingCadence * 7),
                FeedingCadenceType.Months => LastFeedDate.AddMonths(FeedingCadence),
                _ => null
            };

        public object LogEntry { get; set; }
    }
}