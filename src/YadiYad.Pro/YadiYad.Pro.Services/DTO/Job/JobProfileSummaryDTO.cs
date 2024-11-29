using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Common;

namespace YadiYad.Pro.Services.DTO.Job
{
    public class JobProfileSummaryDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string JobTitle { get; set; }
        public bool IsImmediate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ViewJobCandidateFullProfileSubscriptionEndDate { get; set; }
        public double ViewJobCandidateFullProfileSubscriptionEndDays { get; set; }
        public int UnderConsiderationCount { get; set; }
        public int HiredCount { get; set; }
        public int ShortlistedCount { get; set; }
        public bool IsAttentionRequired { get; set; }
    }
}