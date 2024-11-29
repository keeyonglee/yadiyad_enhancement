using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Service
{
    public enum ServiceType
    {
        [Display(Name = "Freelance (Hourly)")]
        [Description("Freelance (Hourly)")]
        Freelancing = 1,
        [Display(Name = "Freelance (Daily)")]
        [Description("Freelance (Daily)")]
        PartTime = 2,
        [Display(Name = "Consultation")]
        [Description("Consultation")]
        Consultation = 3,
        [Display(Name = "Project-based")]
        [Description("Project-based")]
        ProjectBased = 4
    }

    public enum ServiceTypeService
    {
        [Description("Freelance (Hourly)")]
        Freelancing = 1,
        [Description("Consultation")]
        Consultation = 3
    }

    public enum ServiceModel
    {
        [Description("Onsite")]
        Onsite = 1,
        [Description("Partial onsite")]
        PartialOnsite = 2,
        [Description("Remote")]
        Remote = 3
    }

    public enum ServiceAvailabilityBasis
    {
        [Description("Hours Per Day")]
        HoursPerDay = 1,
        [Description("Days Per Week")]
        DaysPerWeek = 2
    }

    public enum ServiceFeeBasis
    {
        [Description("Per Session")]
        PerSession = 1,
        [Description("Per Hours")]
        PerHour = 2,
        [Description("Per Day")]
        PerDay = 3,
        [Description("Per Month")]
        PerMonth = 4
    }

    public enum ServiceApplicationStatus
    {
        [Description("Pending")]
        New = 1,
        [Description("Reproposed Start Date")]
        Reproposed = 2,
        [Description("Accepted")]
        Accepted = 3,
        [Description("Rejected")]
        Rejected = 4,
        [Description("Paid")]
        Paid = 5,
        [Description("Completed")]
        Completed =6,
        [Description("Cancelled By Buyer")]
        CancelledByBuyer = 7,
        [Description("Cancelled By Seller")]
        CancelledBySeller = 8

    }
    public enum ServiceSearchSortBy
    {
        [Description("Price: Low to High")]
        PriceLowToHigh = 1,
        [Description("Price: High to Low")]
        PriceHighToLow = 2,
    }
}
