namespace YadiYad.Pro.Core.Domain.Job
{
    public class ExtendPviEvent
    {
        public readonly int OrgCustomerId;
        public readonly int ServiceSubscriptionId;

        public ExtendPviEvent(int orgCustomerId, int serviceSubscriptionId)
        {
            OrgCustomerId = orgCustomerId;
            ServiceSubscriptionId = serviceSubscriptionId;
        }
    }
}