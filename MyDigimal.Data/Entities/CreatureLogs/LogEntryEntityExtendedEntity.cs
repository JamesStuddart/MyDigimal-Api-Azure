using System;

namespace MyDigimal.Data.Entities.CreatureLogs;

public class LogEntryEntityExtendedEntity : LogEntryEntity
{
    public Guid ParentLogSchemaEntryId { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public bool IsParent { get; set; }
    public int ChartType { get; set; }
}