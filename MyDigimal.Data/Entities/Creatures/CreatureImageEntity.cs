using System;

namespace MyDigimal.Data.Entities.Creatures
{
    public class CreatureImageEntity
    {
        public Guid Id  { get; set; }
        public Guid CreatureId  { get; set; }
        public string Image  { get; set; }
        public DateTime Date  { get; set; }
    }
}