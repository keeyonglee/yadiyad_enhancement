using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Payout
{
    public enum PayoutRequestStatus
    {
        [Description("Draft")]
        Draft = -3,
        [Description("Required More Info")]
        RequiredMoreInfo = -2,
        [Description("Error")]
        Error = -1,
        [Description("New")]
        New = 0,
        [Description("Approved")]
        Approved = 1,
        [Description("Paid")]
        Paid = 2
    }

    public enum PayoutAndGroupRefType
    {
        [Description("PRO Payout")]
        PayoutRequest = 1,
        [Description("PRO Refund")]
        RefundRequest = 2,
        [Description("SHUQ Payout")]
        OrderPayoutRequest = 3,
        [Description("SHUQ Refund")]
        OrderRefundRequest = 4
    }
    public enum PayoutBatchStatus
    {
        [Description("Fail")]
        Fail = -1,
        [Description("New")]
        New = 0,
        [Description("Downloaded")]
        Downloaded = 1,
        [Description("Processing Recon")]
        Processing = 2,
        [Description("Recon Successful")]
        Success = 3,
        [Description("Recon with Error")]
        Error = 4
    }

    public enum PayoutGroupStatus
    {
        [Description("Pending")]
        New = 0,
        [Description("Completed")]
        Success = 1,
        [Description("Unsuccessful")]
        Error = -1
    }

    public enum PayoutBatchGenerationStatus
    {
        [Description("Success")]
        Success = 1,
        [Description("NoItemToProcess")]
        NoItemToProcess = 2,
        [Description("InProcess")]
        InProcess = 3
    }

    public enum Platform
    {
        Pro = 1,
        Shuq = 2
    }

    public enum PayoutReconciliationStatus
    {
        [Description("Success")]
        Success = 1,
        [Description("NoItemToProcess")]
        NoItemToProcess = 2,
        [Description("Failure")]
        Failure = 3
    }
}
