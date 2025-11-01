namespace MyDigimal.Api.Azure.Models;

public class LogEntryViewModel
{
    public Guid CreatureId { get; set; }
    public Guid LogSchemaEntryId { get; set; }
    public DateTime? Date { get; set; }
    public string? Notes { get; set; }
    public string? Value { get; set; }
}