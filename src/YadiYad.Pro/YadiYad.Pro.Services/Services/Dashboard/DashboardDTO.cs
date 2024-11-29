using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Refund;

namespace YadiYad.Pro.Services.Services.Dashboard
{
    public class DashboardJobEngagementDTO
    {
        public int EngagementId { get; set; }
        public DateTime EngagementDate { get; set; }
        public string EngagementTitle { get; set; }
        public int JobApplicationStatus { get; set; }
        public int DepositStatusCondition { get; set; }
        public string EngagementStatusText
        {
            get
            {
                var text =
                DepositStatusCondition == 1
                ? JobEngagementStatus.New.GetDescription()
                : JobApplicationStatus == (int)JobEngagementStatus.New
                ? JobEngagementStatus.New.GetDescription()
                : JobApplicationStatus == (int)JobEngagementStatus.KeepForFutureReference
                ? JobEngagementStatus.KeepForFutureReference.GetDescription()
                : JobApplicationStatus == (int)JobEngagementStatus.Matched
                ? JobEngagementStatus.Matched.GetDescription()
                : JobApplicationStatus == (int)JobEngagementStatus.InvitationRejected
                ? JobEngagementStatus.InvitationRejected.GetDescription()
                : JobApplicationStatus == (int)JobEngagementStatus.CancelledByOrganization
                ? JobEngagementStatus.CancelledByOrganization.GetDescription()
                : JobApplicationStatus == (int)JobEngagementStatus.CancelledByIndividual
                ? JobEngagementStatus.CancelledByIndividual.GetDescription()
                : JobApplicationStatus == (int)JobEngagementStatus.PendingPaymentVerification
                ? JobEngagementStatus.PendingPaymentVerification.GetDescription()
                : JobApplicationStatus == (int)JobEngagementStatus.RevisePaymentRequired
                ? JobEngagementStatus.RevisePaymentRequired.GetDescription()
                : JobApplicationStatus == (int)JobEngagementStatus.Completed
                ? JobEngagementStatus.Completed.GetDescription()
                : JobApplicationStatus == (int)JobEngagementStatus.RefundInitialized
                ? JobEngagementStatus.RefundInitialized.GetDescription()
                : JobApplicationStatus == (int)JobEngagementStatus.Refunded
                ? JobEngagementStatus.Refunded.GetDescription()
                : JobApplicationStatus == (int)JobEngagementStatus.Rematched
                ? JobEngagementStatus.Rematched.GetDescription()
                : "Unknown";

                return text;
            }
        }
        public string DepositStatus 
        { 
            get
            {
                var jobApplicationStatus = new int[] { 12, 13, 14, 15, 17, 18, 19 };
                int index = Array.IndexOf(jobApplicationStatus, JobApplicationStatus);

                var text = IsEscrow == false
                    ? "Non Escrow"
                    : index < -1
                    ? "-"
                    : DepositStatusCondition == 1
                    ? "Rematching"
                    : "-";

                return text;
            } 
        }
        public int Status { get; set; }
        public DateTime? StartDate { get; set; }
        public bool IsEscrow { get; set; }
        public decimal OffsettedAmount { get; set; }
        public int OffsetEngagementId { get; set; }
        public decimal RefundAmount { get; set; }
        public int RefundStatus { get; set; }
        public decimal TotalDepositAmount { get; set; }
        public decimal? TotalPayoutAmount { get; set; }
        public decimal? DepositReserve 
        { 
            get
            {
                var depoAmt = TotalDepositAmount != 0 ? TotalDepositAmount : 0;
                var payoutAmt = TotalPayoutAmount == null ? 0 : TotalPayoutAmount;

                var finalAmt = depoAmt - payoutAmt;

                return finalAmt;
            }
        }
    }

    public class DashboardConsultationEngagementDTO
    {
        public int EngagementId { get; set; }
        public DateTime EngagementDate { get; set; }
        public string Segment { get; set; }
        public int ConsultationRequest { get; set; }
        public int ConsultationApplicationStatus { get; set; }
        public int DepositStatusCondition { get; set; }
        public string EngagementStatusText
        {
            get
            {
                var text =
                DepositStatusCondition == 1
                ? ConsultationEngagementStatus.Matching.GetDescription()
                : ConsultationApplicationStatus == (int)ConsultationEngagementStatus.Matching
                ? ConsultationEngagementStatus.Matching.GetDescription()
                : ConsultationApplicationStatus == (int)ConsultationEngagementStatus.Declined
                ? ConsultationEngagementStatus.Declined.GetDescription()
                : ConsultationApplicationStatus == (int)ConsultationEngagementStatus.Paid
                ? ConsultationEngagementStatus.Paid.GetDescription()
                : ConsultationApplicationStatus == (int)ConsultationEngagementStatus.Completed
                ? ConsultationEngagementStatus.Completed.GetDescription()
                : ConsultationApplicationStatus == (int)ConsultationEngagementStatus.CancelledByOrganization
                ? ConsultationEngagementStatus.CancelledByOrganization.GetDescription()
                : ConsultationApplicationStatus == (int)ConsultationEngagementStatus.CancelledByConsultant
                ? ConsultationEngagementStatus.CancelledByConsultant.GetDescription()
                : ConsultationApplicationStatus == (int)ConsultationEngagementStatus.RefundInitialized
                ? ConsultationEngagementStatus.RefundInitialized.GetDescription()
                : ConsultationApplicationStatus == (int)ConsultationEngagementStatus.Refunded
                ? ConsultationEngagementStatus.Refunded.GetDescription()
                : ConsultationApplicationStatus == (int)ConsultationEngagementStatus.Rematched
                ? ConsultationEngagementStatus.Rematched.GetDescription()
                : "Unknown";

                return text;
            }
        }
        public string DepositStatus
        {
            get
            {
                var consultationApplicationStatus = new int[] { 12, 13, 14, 15, 17, 18, 19 };
                int index = Array.IndexOf(consultationApplicationStatus, ConsultationApplicationStatus);

                var text = index < -1
                    ? "-"
                    : DepositStatusCondition == 1
                    ? "Rematching"
                    : "-";

                return text;
            }
        }
        public int Status { get; set; }
        public DateTime? AppointmentStartDate { get; set; }
        public bool IsEscrow { get; set; }
        public decimal OffsettedAmount { get; set; }
        public int OffsetEngagementId { get; set; }
        public decimal RefundAmount { get; set; }
        public int? RefundId { get; set; }
        public int RefundStatus { get; set; }
        public string RefundStatusText
        {
            get
            {
                var text = RefundId != null
                    ? DashboardRefundStatus.Refunding.GetDescription()
                    : RefundStatus == (int)DashboardRefundStatus.Refunding
                    ? DashboardRefundStatus.Refunding.GetDescription()
                    : RefundStatus == (int)DashboardRefundStatus.Refunded
                    ? DashboardRefundStatus.Refunded.GetDescription()
                    : RefundStatus == (int)DashboardRefundStatus.ReprocessingRefunded
                    ? DashboardRefundStatus.ReprocessingRefunded.GetDescription()
                    : "";

                return text;
            }
        }
        public decimal TotalDepositAmount { get; set; }
        public decimal? TotalPayoutAmount { get; set; }
        public decimal? DepositReserve
        {
            get
            {
                var depoAmt = TotalDepositAmount != 0 ? TotalDepositAmount : 0;
                var payoutAmt = TotalPayoutAmount == null ? 0 : TotalPayoutAmount;

                var finalAmt = depoAmt - payoutAmt;

                return finalAmt;
            }
        }
    }

    public class DashboardCardDTO
    {
        public int? NoOfJobPendingRematch { get; set; }
        public int? NoOfConsultantJobPendingRematch { get; set; }
        public decimal? DepositReserve { get; set; }
        public decimal? DepositDue { get; set; }
        public int? NewApplicant { get; set; }
        public int? JobEngagementCount { get; set; }
        public int? ConsultationEngagementCount { get; set; }
    }
}
