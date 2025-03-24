using System;

namespace MyDigimal.Data.Entities.CreatureLogs
{
    public class LogSchemaEntryEntity
    {
        public Guid Id { get; set; }
        public string Icon { get; set; }
        public Guid SchemaId { get; set; }
        public Guid? ParentId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int Index { get; set; }
        public int ChartType { get; set; }
        public string Values { get; set; }
        public string DefaultValue { get; set; }
        public bool Required { get; set; }
        public bool RepeatLastEntry { get; set; }
        public bool QuickAction { get; set; }
    }
}