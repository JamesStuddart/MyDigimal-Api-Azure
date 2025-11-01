using MyDigimal.Data.Entities.CreatureLogs;
using Newtonsoft.Json;

namespace MyDigimal.Api.Azure.Models;

public class LogEntriesViewModel
{
    [JsonProperty("entries")] public IEnumerable<LogEntryViewModel> Entries { get; set; } = new List<LogEntryViewModel>();

    internal bool IsValid => Entries.Any();
}