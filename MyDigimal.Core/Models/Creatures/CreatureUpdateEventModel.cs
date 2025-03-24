using System;

namespace MyDigimal.Core.Models.Creatures
{
    public class CreatureUpdateEventModel
    {
        public Guid CreatureId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public DateTime EventDate { get; set; }
        public string CreatedBy { get; set; }
    }
}