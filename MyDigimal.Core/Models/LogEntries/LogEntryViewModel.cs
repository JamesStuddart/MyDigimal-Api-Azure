using System.Collections.Generic;
using System.Linq;

namespace MyDigimal.Core.Models.LogEntries
{
    public class LogEntryViewModel
    {
        public IEnumerable<LogEntryDetailModel> Entries { get; set; } = [];
        public IEnumerable<int> AvailableYears { get; set; } = [];
    }
    
}