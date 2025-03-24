using MyDigimal.Common;

namespace MyDigimal.Api.Azure.Models
{
    public class CreatureResponseViewModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string CommonName { get; set; }
        public string Species { get; set; }
        public string Morph { get; set; }
        public string Genes { get; set; }
        public Sex Sex { get; set; }
        public bool WildCaught { get; set; }
        public DateTime? Born { get; set; }
        public string BornYear { get; set; }
        public string BornBy { get; set; }
        public string BornByUrl { get; set; }
        public Guid? SireId { get; set; }
        public Guid? DamId { get; set; }
        public DateTime? PurchasedOn { get; set; }
        public string PurchasedFrom { get; set; }
        public string PurchasedFromUrl { get; set; }
        public CreatureStatus Status { get; set; }
        public Guid LogSchemaId { get; set; }
        public Guid Owner { get; set; }
        public string Image { get; set; }
        public string ShortCode { get; set; }
        public int FeedingCadence { get; set; } 
        public FeedingCadenceType FeedingCadenceType { get; set; }
        public string Tags { get; set; }
        public string GroupName { get; set; }
    }
}