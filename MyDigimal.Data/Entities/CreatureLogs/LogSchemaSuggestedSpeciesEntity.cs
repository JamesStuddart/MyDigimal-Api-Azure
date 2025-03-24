using System;

namespace MyDigimal.Data.Entities.CreatureLogs
{
    public class LogSchemaSuggestedSpeciesEntity
    {
        public Guid Id { get; set; }
        public Guid LogSchemaId { get; set; }
        public Guid SuggestedSpeciesId { get; set; }
    }
}