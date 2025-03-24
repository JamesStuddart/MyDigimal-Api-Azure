using System;

namespace MyDigimal.Data.Entities.Creatures
{
    public class SuggestedSpeciesEntity
    {
        public Guid Id { get; set; }
        public string SpeciesName { get; set; }
        public string CommonNames { get; set; }
    }
}