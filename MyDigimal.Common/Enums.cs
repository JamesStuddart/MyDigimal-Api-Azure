using System.ComponentModel;

namespace MyDigimal.Common
{
    public enum Sex
    {
        Unknown = 0,
        Male = 1000,
        Female = 2000
    }

    public enum CreatureStatus
    {
        Archived = -1,
        Unknown = 0,
        Alive = 1000,
        Missing = 2000,
        Sick = 3000,
        Dead = 9000,
    }

    public enum FeedingCadenceType
    {
        NotSet = 0,
        Hours = 1000,
        Days = 2000,
        Weeks = 3000,
        Months = 4000
    }
    
    public enum ChartType
    {
        None = 0,
        HeatMap = 1000,
        Line = 2000,
    }

    public enum AccountPlanType
    {
        [Description("")]
        Disabled = 0,
        [Description("")]
        Free = 1000,
        [Description("")]
        Starter = 2000,
        [Description("")]
        Hobbiest = 3000,
        [Description("")]
        Breeder = 4000,
        [Description("")]
        Ultra = 10000,
        [Description("")]
        EarlyAdopter = 20000
    }

    public enum PaymentPlanType
    {
        [Description("")]
        Monthly = 0,
        [Description("")]
        Yearly = 1000
    }

    public enum AccountStatusType
    {
        Preregister = -1000,
        Active = 0,
        Suspended = 1000,
        Deactivated = 2000,
        DeactivatedByUser = 2100,
        Expired = 2200,
        ExpiredDueToNonPayment = 2210,
    }

    public enum AccountRoleType
    {
        Public = -1000,
        User = 0,
        Admin = 1000,
    }
    
    public enum CreatureEventType
    {
        Generic = 0,
        Created = 1000,
        OwnerChange = 2000,
        StatusChange = 3000
    }

    public enum NotificationType
    {
        General = 0,
        //Account
        SubscriptionActivated = 1000,
        SubscriptionChanged = 1050,
        SubscriptionExpiring = 1100,
        SubscriptionExpired = 1150,
        SubscriptionCancelled = 1500,
        //Animal Transfers
        DigimalTransferredTo = 2000,
        DigimalTransferredFrom = 2100,
        DigimalTransferFailed = 2200,
        DigimalAwaitingTransferTo = 2300,
        //Messaging
        MessageArrived = 3000,
        MessageSent = 3500,
        //Website 
        WebsiteCreated = 4000,
        WebsiteDeleted = 4100,
        WebsiteLive = 4200,
        WebsiteOffline = 4300,
        WebsiteSuspended = 4400,
        //Website Builds
        WebsiteBuildRequested = 5000,
        WebsiteBuildFailed = 5100,
        WebsiteBuildCancelled = 5200,
        WebsiteBuildCancelledBySystem = 5250,
        WebsiteBuildCompleted = 5300,
        WebsiteDelopymentStarted = 5400,
        WebsiteDelopymentFailed = 5500,
        WebsiteDelopymentCompleted = 5600,
        //DomainName
        DomainPurchased = 6000,
        DomainExpired = 6100,
        DomainRenewed = 6200,
    }

    public enum NotificationProcessingType
    {
        Accept = 0,
        Decline = 1000
    }
}