using System.ComponentModel;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents a return status
    /// </summary>
    public enum DisputeActionEnum
    {
        Pending = 0,

        FullRefundFromBuyer = 10,

        PartialRefund = 20,

        NoRefund = 30,
    }
}
