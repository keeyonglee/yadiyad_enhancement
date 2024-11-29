using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Campaign
{
    public enum CampaignChannel
    {
        Pro = 1,
        Shuq = 2
    }
    public enum CampaignActivityIndividual
    {
        [Display(Name = "Registration")]
        [Description("Registration")]
        Registration = 1,
        [Display(Name = "Registration Referral")]
        [Description("Registration Referral")]
        RegistrationReferral = 2,
        [Display(Name = "Payout")]
        [Description("Payout")]
        Payout = 3,
        [Display(Name = "Submit Rating")]
        [Description("Submit Rating")]
        SubmitRating = 4,
    }
    public enum CampaignActivityOrganization
    {
        [Display(Name = "New Job Post")]
        [Description("New Job Post")]
        NewJobPost = 1,
        [Display(Name = "Extend Job Post")]
        [Description("Extend Job Post")]
        ExtendJobPost = 2,
        [Display(Name = "Pay To View")]
        [Description("Pay To View")]
        PayToView = 3,
        [Display(Name = "Submit Rating")]
        [Description("Submit Rating")]
        SubmitRating = 4,
    }
    public enum CampaignEngagementType
    {
        Job = 1,
        Service = 2,
        Consultation = 3,
    }
    public enum CampaignType
    {
        [Display(Name = "Pay-to-Apply Jobs")]
        [Description("Pay-to-Apply Jobs")]
        PayToApplyJobs =1,
        [Display(Name = "Pay-to-View-and-Invite")]
        [Description("Pay-to-View-and-Invite")]
        PayToViewAndInvite = 2,
        [Display(Name = "Credit Voucher")]
        [Description("Credit Voucher")]
        CreditVoucher = 3,
        [Display(Name = "Charges Waiver")]
        [Description("Charges Waiver")]
        ChargesWaiver = 4,
        [Display(Name = "Credit Voucher Referral")]
        [Description("Credit Voucher Referral")]
        CreditVoucherReferral = 5,
        [Display(Name = "Extend-Pay-to-View-and-Invite")]
        [Description("Extend-Pay-to-View-and-Invite")]
        ExtendPayToViewAndInvite = 6,
    }
    public enum CampaignBeneficiary
    {
        Organization = 1,
        Individual = 2
    }
    
    public enum CampaignProcessType
    {
        Process,
        Apply,
    }
}
