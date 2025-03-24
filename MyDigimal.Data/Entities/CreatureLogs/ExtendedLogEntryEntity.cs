using System;

namespace MyDigimal.Data.Entities.CreatureLogs
{
    public class ExtendedLogEntryEntity
    {
        public Guid Id { get; set; }
        public Guid LogEntryId { get; set; }
        public Guid LogSchemaEntryId { get; set; }
        public string Value { get; set; }
    }
}