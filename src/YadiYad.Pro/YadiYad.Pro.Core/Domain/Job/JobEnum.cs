using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Job
{
    public enum JobProfileStatus
    {
        Draft = 0,
        Publish = 1,
        Hired = 2
    }

    public enum JobType
    {
        [Display(Name = "Freelance (Hourly)")]
        [Description("Freelance (Hourly)")]
        Freelancing = 1,
        [Display(Name = "Freelance (Daily)")]
        [Description("Freelance (Daily)")]
        PartTime = 2,
        [Display(Name = "Project-based")]
        [Description("Project-based")]
        ProjectBased = 3
    }

    public enum JobModel
    {
        [Description("Onsite")]
        Onsite = 1,
        [Description("Partial onsite")]
        PartialOnsite = 2,
        [Description("Remote")]
        Remote = 3
    }

    public enum JobDurationPhase
    {
        [Description("hours per day")]
        HoursPerWeek,
        [Description("days per week")]
        DaysPerWeek,
        [Description("months")]
        Months
    }
    public enum JobPaymentPhase
    {
        [Description("per hour")]
        PerHour = 1,
        [Description("per month")]
        PerMonth,
        [Description("total job remuneration")]
        TotalJob
    }

    public enum JobApplicationStatus
    {
        [Description("New")]
        New = 1,
        [Description("Shortlisted")]
        Shortlisted = 2,
        [Description("Interview")]
        Interview = 3,
        [Description("Under Consideration")]
        UnderConsideration = 4,
        [Description("KIV")]
        KeepForFutureReference = 5,
        [Description("Hired")]
        Hired = 6,
        //[Description("Paid")]
        //Paid = 7,
        //[Description("Not Hired")]
        //NotHired = 8,
        [Description("Rejected")]
        Rejected = 9,
        //[Description("Rejected")]
        //RejectedByJobSeeker = 10,
        [Description("Expired")]
        Expired = 11,
        [Description("Cancelled By Organization")]
        CancelledByOrganization = 12,
        [Description("Cancelled By Job Seeker")]
        CancelledByIndividual = 13,
        [Description("Pending Payment Verification")]
        PendingPaymentVerification = 14,
        [Description("Revise Payment Required")]
        RevisePaymentRequired = 15,
        [Description("Completed")]
        Completed = 16
    }

    public enum JobInvitationStatus
    {
        Pending = 1,
        Accepted = 2,
        Declined = 3,
        Expired = 4,
        Reviewing = 5,
        UpdateRequired = 6
    }

    public enum JobSchedule
    {
        [Description("Half Day")]
        HalfDaily = 2,
        [Description("Full Day")]
        Daily = 1
    }

    public enum JobCancellationStatus
    {
        FirstCancellationProceedRehire = 1,
        FirstCancellationProceedRefund = 2,
        SecondCancellationDepositCollected = 3,
        SecondCancellationDepositNotCollected = 4,
        FirstCancellationAlreadyRehired = 5,
    }
}
