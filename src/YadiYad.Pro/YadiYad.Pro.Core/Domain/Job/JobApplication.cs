using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Core.Domain.Job
{
    public class JobApplication : BaseEntityExtension
    {
        public int JobSeekerProfileId { get; set; }
        public int JobProfileId { get; set; }
        public int OrganizationProfileId { get; set; }
        public bool IsRead { get; set; }
        public bool IsOrganizationRead { get; set; }
        public int? JobInvitationId { get; set; }
        public int JobApplicationStatus { get; set; }
        public bool IsEscrow { get; set; }

        public decimal PayAmount { get; set; }
        public int JobType { get; set; }
        public int? JobRequired { get; set; }

        public decimal? Rating { get; set; }
        public decimal? KnowledgenessRating { get; set; }
        public decimal? RelevanceRating { get; set; }
        public decimal? RespondingRating { get; set; }
        public decimal? ClearnessRating { get; set; }
        public decimal? ProfessionalismRating { get; set; }
        public string ReviewText { get; set; }
        public DateTime? ReviewDateTime { get; set; }

        public int? CancellationReasonId { get; set; }
        public string CancellationRemarks { get; set; }
        public int? CancellationDownloadId { get; set; }
        public DateTime? CancellationDateTime { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? EndMilestoneId { get; set; }

        public int? RehiredobApplicationId { get; set; }
        public DateTime? HiredTime { get; set; }

        [ForeignKey("CancellationReasonId")]
        public Reason Reason { get; set; }
        public int NumberOfHiring { get; set; }
    }
}
