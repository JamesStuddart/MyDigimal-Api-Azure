using System;

namespace MyDigimal.Data.Entities.CreatureLogs
{
    public class LogEntryEntity
    {
        public Guid Id { get; set; }
        public Guid CreatureId { get; set; }
        public Guid LogSchemaEntryId { get; set; }
        public DateTime? Date { get; set; }
        public string Notes { get; set; }
        public string Value { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid Owner { get; set; }
    }
}