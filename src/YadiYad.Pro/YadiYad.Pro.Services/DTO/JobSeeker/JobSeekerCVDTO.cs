using System;

namespace YadiYad.Pro.Services.DTO.JobSeeker
{
    public class JobSeekerCVDTO
    {
        public int JobSeekerProfileId { get; set; }
        public int Id { get; set; }
        public int? NewDownloadId { get; set; }
        public int DownloadId { get; set; }
        public Guid? DownloadGuid { get; set; }
        public string DownloadName { get; set; }
    }
}