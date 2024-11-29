using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Home
{
    public enum ContactUsType
    {
        Enquiry = 1,
        Issues = 2,
        Feedbacks = 3
    }

    public enum ContactUsSubject
    {
        [Display(Name = "Cancellation & Refund")]
        [Description("Cancellation & Refund")]
        CancellationAndRefund = 1,
        [Display(Name = "Collaboration")]
        [Description("Collaboration")]
        Collaboration = 2,
        [Display(Name = "Complains")]
        [Description("Complains")]
        Complains = 3,
        [Display(Name = "Others")]
        [Description("Others")]
        Others = 4,
        [Display(Name = "Dispute")]
        [Description("Dispute")]
        Dispute = 5,
        [Display(Name = "Feedbacks")]
        [Description("Feedbacks")]
        Feedbacks = 6,
        [Display(Name = "General Enquiry")]
        [Description("General Enquiry")]
        GeneralEnquiry = 7,
        [Display(Name = "Suggestions")]
        [Description("Suggestions")]
        Suggestions = 8,
        [Display(Name = "Testimonials")]
        [Description("Testimonials")]
        Testimonials = 9,
        [Display(Name = "CSR")]
        [Description("CSR")]
        CSR = 10
    }
}
