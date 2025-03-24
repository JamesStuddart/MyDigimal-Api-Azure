using System;
using System.Collections.Generic;
using MyDigimal.Common;

namespace MyDigimal.Core.Models.LogSchema
{
    public class LogSchemaEntriesModel
    {
        public Guid Id { get; set; }
        public Guid SchemaId { get; set; }
        public Guid? ParentId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int Index { get; set; }
        public ChartType ChartType { get; set; }
        public IEnumerable<string> Values { get; set; }
        public string DefaultValue { get; set; }
        public bool Required { get; set; }
        public IEnumerable<LogSchemaEntriesModel> ChildEntries { get; set; }
        public bool RepeatLastEntry { get; set; }
        public string Icon { get; set; }
        public bool QuickAction { get; set; }
    }
}