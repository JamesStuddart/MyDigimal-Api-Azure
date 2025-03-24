using System;

namespace MyDigimal.Data.Entities.Creatures
{
    public class CreatureNoteEntity
    {
        public Guid Id { get; set; }
        public Guid CreatureId { get; set; }
        public string Notes { get; set; }
        public Guid CreatedBy { get; set; }
    }
}