using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Common;

namespace YadiYad.Pro.Services.DTO.Job
{
    public class JobProfileDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        [Required(ErrorMessage = "Please select category")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Please insert job title")]
        public string JobTitle { get; set; }
        [Required(ErrorMessage = "Please select preferred experience")]
        public int PreferredExperience { get; set; }
        [Required(ErrorMessage = "Please select job type")]
        public int JobType { get; set; }
        public int JobSchedule { get; set; }
        public int? JobRequired { get; set; }
        //[Required(ErrorMessage = "Please select job duration phase")]
        //public int JobDurationPhase { get; set; }
        public bool IsImmediate { get; set; }
        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage = "Please select is onsite")]
        public bool IsOnsite { get; set; }
        [Required(ErrorMessage = "Please select is partial onsite")]
        public bool IsPartialOnsite { get; set; }
        [Required(ErrorMessage = "Please select is remote")]
        public bool IsRemote { get; set; }
        public int? CityId { get; set; }
        public int? StateProvinceId { get; set; }
        public int? CountryId { get; set; }
        [Required(ErrorMessage = "Please insert pay amount")]
        public decimal PayAmount { get; set; }
        //[Required(ErrorMessage = "Please select job payment phase")]
        //public int JobPaymentPhase { get; set; }
        [Required(ErrorMessage = "Please select interest hobby")]
        public List<ExpertiseDTO> RequiredExpertises { get; set; }


        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }


        public string CategoryName { get; set; }
        public string JobTypeText { 
            get
            {
                string text = null;
                var jobType = (Core.Domain.Job.JobType?)JobType;

                if (jobType != null)
                {
                    text = jobType.GetDescription();
                }

                return text;
            }
        }
        public string JobScheduleText
        {
            get
            {
                string text = null;
                var jobSchedule = (Core.Domain.Job.JobSchedule?)JobSchedule;

                if (jobSchedule != null)
                {
                    text = jobSchedule.GetDescription();
                }

                return text;
            }
        }
        public string PreferredExperienceName { get; set; }
        //public string JobDurationPhaseName { get; set; }
        //public string JobPaymentPhaseName { get; set; }
        public string CityName { get; set; }
        public string StateProvinceName { get; set; }
        public string CountryName { get; set; }
        public string OrganizationName { get; set; }
        public bool Deleted { get; set; }
        public DateTime? ViewJobCandidateFullProfileSubscriptionEndDate { get; set; }
        public double ViewJobCandidateFullProfileSubscriptionEndDays { get; set; } = 0;
        public int Status { get; set; }

        public List<JobMilestoneDTO> JobMilestones { get; set; } = new List<JobMilestoneDTO>();


        public string Code
        {
            get
            {
                return "YP" + ("00000" + Id).PadRight(4);
            }
        }

        [JsonIgnore]
        public decimal FeePerDepositRequest
        {
            get
            {
                decimal fee = 0;

                //check type
                if (this.JobType == (int)YadiYad.Pro.Core.Domain.Job.JobType.Freelancing)
                {
                    //by week * 4 = month
                    fee = this.PayAmount * this.JobRequired.Value * 4;
                }
                else if (this.JobType == (int)YadiYad.Pro.Core.Domain.Job.JobType.PartTime)
                {
                    //by month
                    fee = this.PayAmount * this.JobRequired.Value;
                }
                else if (this.JobType == (int)YadiYad.Pro.Core.Domain.Job.JobType.ProjectBased)
                {
                    //by session or by project
                    fee = this.PayAmount;
                }

                return fee;
            }
        }
    }
}