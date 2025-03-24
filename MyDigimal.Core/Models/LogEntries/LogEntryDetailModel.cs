using System;
using System.Collections.Generic;
using System.Linq;
using MyDigimal.Common;

namespace MyDigimal.Core.Models.LogEntries
{
    public class LogEntryDetailModel
    {
        public Guid? Id { get; set; }
        public Guid? LogSchemaEntryId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public Guid? CreatureId { get; set; }
        public string Notes { get; set; }
        public string Value { get; set; }
        public string Owner { get; set; }
        public string Date { get; set; }
        public ChartType ChartType { get; set; }
        public IEnumerable<LogEntryDetailModel> LogEntries { get; set; }
        public IOrderedEnumerable<int> availableYears { get; set; }
    }
}