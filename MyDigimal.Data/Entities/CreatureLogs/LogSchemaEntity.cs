using System;

namespace MyDigimal.Data.Entities.CreatureLogs
{
    public class LogSchemaEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool IsPublic { get; set; }
        public Guid Author { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }
        public string RecommendedSpecies { get; set; }
        public string RecommendedCommonNames { get; set; }
        public string Genes { get; set; }
        public string Morphs { get; set; }
    }
}