using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Order
{
    public enum ProductType
    {

        [Display(Name = "Shuq Order Payout Service Fee")]
        [Description("Shuq order payout fee charge by YadiYad Shuq platform.")]
        ShuqOrderPayoutServiceFee = 0,

        [Display(Name = "Pay-to-Apply Jobs (PAJ)")]
        [Description("Pay-to-Apply Jobs (PAJ)")]
        ApplyJobSubscription = 1,

        [Display(Name = "Pay-to-View-and-Invite (PVI)")]
        [Description("Pay-to-View-and-Invite (PVI)")]
        ViewJobCandidateFullProfileSubscription = 2,

        //consultation
        [Display(Name = "Consultation Engagement Matching Fee")]
        [Description("Consultation engagement matching fee charge by YadiYad Pro platform.")]
        ConsultationEngagementMatchingFee = 3,

        [Display(Name = "Consultation Engagement Fee")]
        [Description("Consultation engagement fee charge on behalf of consultant.")]
        ConsultationEngagementFee = 31,

        [Display(Name = "Consultation Escrow Fee")]
        [Description("Consultation escrow fee charge by YadiYad Pro platform.")]
        ConsultationEscrowFee = 32,

        [Display(Name = "Consultation Buyer Cancellation Admin Charges")]
        [Description("Admin charges by platform if enagagement cancelled by buyer.")]
        ConsultationBuyerCancellationAdminCharges = 33,

        [Display(Name = "Admin Charges on Consultation Engagement Matching Fee Refund")]
        [Description("Admin charges by platform on consultation Engagement matching fee")]
        ConsultationEngagementMatchingFeeRefundAdminCharges = 34,

        //service
        [Display(Name = "Service Enagegement Matching Fee")]
        [Description("Service enagegement matching fee charge by YadiYad Pro platform.")]
        ServiceEnagegementMatchingFee = 4,

        [Display(Name = "Service Engagement Fee")]
        [Description("Service escrow charge by service provider.")]
        ServiceEnagegementFee = 41,

        [Display(Name = "Service Escrow Fee")]
        [Description("Service escrow fee charge by YadiYad Pro platform.")]
        ServiceEscrowFee = 42,

        [Display(Name = "Service Buyer Cancellation Admin Charges")]
        [Description("Admin charges by platform if enagagement cancelled by buyer.")]
        ServiceBuyerCancellationAdminCharges = 43,

        //job
        [Display(Name = "Job Engagement Fee")]
        [Description("Job engagement fee charge by job seeker.")]
        JobEnagegementFee = 5,

        [Display(Name = "Job Escrow Fee")]
        [Description("Job escrow fee charge by YadiYad Pro platform.")]
        JobEscrowFee = 51,

        [Display(Name = "Job Buyer Cancellation Admin Charges")]
        [Description("Admin charges by platform if enagagement cancelled by buyer.")]
        JobBuyerCancellationAdminCharges = 53,

        //deposit request
        [Display(Name = "Deposit Request")]
        [Description("Deposit Request")]
        DepositRequest = 60,

        [Display(Name = "Moderator Facilitate Consultation Fee")]
        [Description("Moderator facilitate consultation fee charge by moderator.")]
        ModeratorFacilitateConsultationFee = 7,

        [Display(Name = "Refund Professional Fees Deposit")]
        [Description("Refund Professional Fees Deposit")]
        RefundProfessionalFeesDeposit = 8
    }
    public enum ChargeValueType
    {
        Amount = 1,
        Rate = 2,
    }

    public enum InvoiceRefType
    {
        Order = 1,
        Payout = 2
    }

    public enum StatementType
    {
        [Description("Invoice")]
        Invoice = 1,
        [Description("Deposit")]
        Deposit = 2,
        [Description("Refund")]
        Refund = 3,
        [Description("Credit Note")]
        CreditNote = 4
    }

    public enum Account
    {
        PlatformAccount = 1,
        FundHoldingAccount = 2,
    }

    public enum ProOrderItemStatus
    {
        Paid = 0,
        OpenForRematch = 1,
        Remtached = 2
    }

    //public enum OrderStatus
    //{
    //    /// <summary>
    //    /// Pending
    //    /// </summary>
    //    Pending = 10,

    //    /// <summary>
    //    /// Processing
    //    /// </summary>
    //    Processing = 20,

    //    /// <summary>
    //    /// Complete
    //    /// </summary>
    //    Complete = 30,

    //    /// <summary>
    //    /// Cancelled
    //    /// </summary>
    //    Cancelled = 40
    //}
    //public enum PaymentStatus
    //{
    //    /// <summary>
    //    /// Pending
    //    /// </summary>
    //    Pending = 10,

    //    /// <summary>
    //    /// Authorized
    //    /// </summary>
    //    Authorized = 20,

    //    /// <summary>
    //    /// Paid
    //    /// </summary>
    //    Paid = 30,

    //    /// <summary>
    //    /// Partially Refunded
    //    /// </summary>
    //    PartiallyRefunded = 35,

    //    /// <summary>
    //    /// Refunded
    //    /// </summary>
    //    Refunded = 40,

    //    /// <summary>
    //    /// Voided
    //    /// </summary>
    //    Voided = 50
    //}
}
