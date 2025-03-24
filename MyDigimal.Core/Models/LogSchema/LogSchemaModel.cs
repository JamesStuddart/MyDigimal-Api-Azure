using System;
using System.Collections.Generic;

namespace MyDigimal.Core.Models.LogSchema
{
    public class LogSchemaModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool IsPublic { get; set; }
        public Guid Author { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }
        public IEnumerable<LogSchemaEntriesModel> Entries { get; set; }
        public string RecommendedSpecies { get; set; }
        public string RecommendedCommonNames { get; set; }
    }
}