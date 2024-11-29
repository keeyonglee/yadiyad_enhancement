using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.DTO.Individual;
using YadiYad.Pro.Services.DTO.JobSeeker;
using YadiYad.Pro.Services.DTO.Questionnaire;
using YadiYad.Pro.Services.DTO.Service;

namespace YadiYad.Pro.Services.DTO.Job
{
    public class JobApplicationDTO
    {
        public int Id { get; set; }

        public int JobSeekerProfileId { get; set; }
        public int ServiceProfileId { get; set; }
        public int JobProfileId { get; set; }
        public int ConsultationProfileId { get; set; }
        public int NumberOfHiring { get; set; }
        public int? ModeratorCustomerId { get; set; }
        public string ModeratorEmail { get; set; }
        public int OrganizationProfileId { get; set; }
        public bool IsRead { get; set; }
        public bool IsOrganizationRead { get; set; }
        public int? JobInvitationId { get; set; }
        public int? JobCancellationStatus { get; set; }
        public int JobApplicationStatus { get; set; }
        public string JobApplicationStatusName
        {
            get
            {
                var name = "";

                if (JobProfileId != 0)
                {
                    JobApplicationStatus? jobApplicationStatus = (JobApplicationStatus)JobApplicationStatus;

                    if (jobApplicationStatus != null)
                    {
                        name = jobApplicationStatus.GetDescription();
                    }
                }
                else if (ConsultationProfileId != 0)
                {
                    ConsultationInvitationStatus? consultationInvitationStatus = (ConsultationInvitationStatus)JobApplicationStatus;

                    if (consultationInvitationStatus != null)
                    {
                        name = consultationInvitationStatus.GetDescription();
                    }
                }

                return name;
            }
        }

        public decimal? Rating { get; set; }
        public decimal? KnowledgenessRating { get; set; }
        public decimal? RelevanceRating { get; set; }
        public decimal? RespondingRating { get; set; }
        public decimal? ClearnessRating { get; set; }
        public decimal? ProfessionalismRating { get; set; }
        public string ReviewText { get; set; }
        public DateTime? ReviewDateTime { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CancellationDateTime { get; set; }
        public int? EndMilestoneId { get; set; }
        public DateTime? CanCancelDate { get; set; }
        public int DaysAbleToCancel { get; set; }
        public bool CanCancel { get; set; }
        public bool CanRefund { get; set; }
        public decimal? RatesPerSession { get; set; }
        public List<TimeSlotDTO> ConsultantAvailableTimeSlots { get; set; } = new List<TimeSlotDTO>();
        public List<QuestionDTO> ConsultantReplys { get; set; } = new List<QuestionDTO>();
        public DateTime? AppointmentStartDate { get; set; }
        public DateTime? AppointmentEndDate { get; set; }
        public string AppointmentStartText { get; set; }
        public string AppointmentEndText { get; set; }
        public string AppointmentDateText { get; set; }
        public string RescheduleRemarks { get; set; }
        public string CancellationEndRemarks { get; set; }

        public ConsultationProfileDTO ConsultationProfile { get; set; }

        public JobProfileDTO JobProfile { get; set; }
        public ServiceProfileDTO ServiceProfile { get; set; }
        public JobSeekerProfileDTO JobSeekerProfile { get; set; }
        public IndividualProfileDTO ServiceIndividualProfile { get; set; }
        public string OrganizationName { get; set; }
        public bool IsEscrow { get; set; }

        public decimal PayAmount { get; set; }
        public int JobType { get; set; }
        public string JobTypeText { get; set; }
        public int? JobRequired { get; set; }

        public List<JobMilestoneDTO> JobMilestones { get; set; } = new List<JobMilestoneDTO>();

        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }

        public string Code
        {
            get
            {
                return "YP" + ("00000" + Id).PadRight(4);
            }
        }

        public bool CanTerminate
        {
            get
            {
                var canTerminate = false;

                if (JobProfileId != 0)
                {
                    canTerminate = JobApplicationStatus == (int)YadiYad.Pro.Core.Domain.Job.JobApplicationStatus.Hired
                    && DateTime.Today >= StartDate
                    && EndDate.HasValue == false 
                    && EndMilestoneId.HasValue == false;
                }

                return canTerminate;
            }
        }

        public bool IsEngagementEnded
        {
            get
            {
                var isEngagementEnded = false;

                isEngagementEnded = EndDate.HasValue == true
                    && DateTime.Today > EndDate;

                return isEngagementEnded;
            }
        }

        public string CreatedOnUTCText 
        {
            get
            {
                return CreatedOnUTC.ToShortDateString();
            }
        }

        [JsonIgnore]
        public decimal FeePerDepositExclSST
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