using MyDigimal.Common;

namespace MyDigimal.Core.AccountPlans.Models
{
    public class AccountPlanModel
    {
        public AccountPlanType AccountPlanType { get; set; }
        
        public int MaxCreatures { get; set; }
        
        public bool GenerateWebsite { get; set; }
        public bool DefineCustomDomain { get; set; }
        
        public bool CustomAnimalCards { get; set; }
        public bool PublicAnimalCards { get; set; }
        public bool PrivateAnimalCards { get; set; }

        public bool CustomSalesCards { get; set; }
        
        public bool CustomQuickActionCards { get; set; }

        public bool CustomSchemas { get; set; }
        
        public bool StoreFileAgainstAnimals { get; set; }
    }
}