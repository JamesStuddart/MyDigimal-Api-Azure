using System;

namespace MyDigimal.Data.Entities.System
{
    public class NewsEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public bool IsArchived { get; set; }
        public Guid Author { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }
    }
}