using System;

namespace MyDigimal.Data.Entities.Creatures
{
    public class CreatureEventEntity
    {
        public Guid CreatureId { get; set; }
        public int Event { get; set; }
        public string ValueName { get; set; }
        public string OriginalValue { get; set; }
        public string NewValue { get; set; }
        public DateTime EventDate { get; set; }
        public Guid CreatedBy { get; set; }
    }
}