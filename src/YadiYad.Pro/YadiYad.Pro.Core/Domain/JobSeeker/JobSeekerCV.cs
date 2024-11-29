using System.ComponentModel.DataAnnotations.Schema;

namespace YadiYad.Pro.Core.Domain.JobSeeker
{
    public class JobSeekerCV: BaseEntityExtension
    {
        public int DownloadId { get; set; }
        public int JobSeekerProfileId { get; set; }

        [ForeignKey("JobSeekerProfileId")]
        public JobSeekerProfile JobSeekerProfile { get; set; }
        
    }
}