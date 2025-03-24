using System.Text.Json.Serialization;

namespace MyDigimal.Api.Azure.Models
{
    public class GeneticsViewModel
    {
        public IEnumerable<Gene> Genes { get; set; }
        public IEnumerable<Morph> Morphs { get; set; }
    }

    public class Gene
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public IEnumerable<string> Genes { get; set; }
        public string Complex { get; set; }
    }
    public class Morph
    {
        public string Name { get; set; }
        public IEnumerable<string> Genes { get; set; }
        public string Complex { get; set; }
        [JsonPropertyName("Known Issues")]
        public string KnownIssues { get; set; }
    }
}