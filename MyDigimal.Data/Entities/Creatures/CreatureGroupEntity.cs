using System;

namespace MyDigimal.Data.Entities.Creatures
{
    public class CreatureGroupEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CreatedBy { get; set; }
    }
}