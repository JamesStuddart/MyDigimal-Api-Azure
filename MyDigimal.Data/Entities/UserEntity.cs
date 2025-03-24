using System;
using MyDigimal.Common;

namespace MyDigimal.Data.Entities
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        
        public AccountPlanType AccountPlan { get; set; } = AccountPlanType.Free;
        public PaymentPlanType PaymentPlan { get; set; } = PaymentPlanType.Monthly;
        public AccountStatusType AccountStatus { get; set; } = AccountStatusType.Active; 
        
        public int? RenewalMonth { get; set; }
        public int? RenewalYear { get; set; }
        
        public AccountRoleType Role { get; set; }
    }
}