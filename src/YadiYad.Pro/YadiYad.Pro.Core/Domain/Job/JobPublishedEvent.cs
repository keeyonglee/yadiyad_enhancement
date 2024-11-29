namespace YadiYad.Pro.Core.Domain.Job
{
    public class JobPublishedEvent
    {
        public readonly int JobProfileId;
        public readonly int OrgCustomerId;
        public JobPublishedEvent(int orgCustomerId, int jobProfileId)
        {
            JobProfileId = jobProfileId;
            OrgCustomerId = orgCustomerId;
        }
    }
}