using MyDigimal.Common;
using MyDigimal.Core.AccountPlans.Models;

namespace MyDigimal.Core.AccountPlans
{
    public interface IAccountPlanFactory
    {
        AccountPlanModel GetModel(AccountPlanType accountPlanType);
    }
}