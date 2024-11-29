using System.ComponentModel;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents a return status
    /// </summary>
    public enum ReturnConditionEnum
    {
        [Description("Pending")]
        Pending = 0,
        [Description("Mint")]
        Mint = 20,
        [Description("Defect (Raise Dispute)")]
        Defect = 30
    }
}
