using MyDigimal.Common;
using MyDigimal.Core.AccountPlans.Models;

namespace MyDigimal.Core.AccountPlans
{
    public class AccountPlanFactory : IAccountPlanFactory
    {
        private AccountPlanModel _breeder = new AccountPlanModel
        {
            MaxCreatures = -1,
            GenerateWebsite = true,
            DefineCustomDomain = true,
            CustomAnimalCards = true,
            PublicAnimalCards = true,
            PrivateAnimalCards = true,
            CustomSalesCards = true,
            CustomQuickActionCards = true,
            CustomSchemas = true,
            StoreFileAgainstAnimals = true
        };
        
        private AccountPlanModel _hobbiest = new AccountPlanModel
        {
            MaxCreatures = 50,
            GenerateWebsite = true,
            DefineCustomDomain = false,
            CustomAnimalCards = true,
            PublicAnimalCards = true,
            PrivateAnimalCards = true,
            CustomSalesCards = true,
            CustomQuickActionCards = true,
            CustomSchemas = true,
            StoreFileAgainstAnimals = true
        };
      
        private AccountPlanModel _starter = new AccountPlanModel
        {
            MaxCreatures = 5,
            GenerateWebsite = false,
            DefineCustomDomain = false,
            CustomAnimalCards = false,
            PublicAnimalCards = false,
            PrivateAnimalCards = true,
            CustomSalesCards = false,
            CustomQuickActionCards = false,
            CustomSchemas = false,
            StoreFileAgainstAnimals = true
        };
        
        private AccountPlanModel _free = new AccountPlanModel
        {
            MaxCreatures = 1,
            GenerateWebsite = false,
            DefineCustomDomain = false,
            CustomAnimalCards = false,
            PublicAnimalCards = false,
            PrivateAnimalCards = true,
            CustomSalesCards = false,
            CustomQuickActionCards = false,
            CustomSchemas = false,
            StoreFileAgainstAnimals = false
        };

        public AccountPlanModel GetModel(AccountPlanType accountPlanType)
        {
            var model = new AccountPlanModel();
            
            switch (accountPlanType)
            {
                case AccountPlanType.Starter:
                    model = _starter;
                    break;
                case AccountPlanType.Hobbiest:
                    model = _hobbiest;
                    break;
                case AccountPlanType.Breeder:
                case AccountPlanType.Ultra:
                case AccountPlanType.EarlyAdopter:
                    model = _breeder;
                    break;
                case AccountPlanType.Free:
                default:
                    model = _free;
                    break;
            }

            model.AccountPlanType = accountPlanType;
            
            return model;
        }
    }
}