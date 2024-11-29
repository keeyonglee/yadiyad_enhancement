using System.ComponentModel;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents a return status
    /// </summary>
    public enum ApproveStatusEnum
    {
        Pending = 10,

        Approved = 20,

        NotApproved = 30,

        InDispute = 40,
    }

    public enum EditApproveStatusEnum
    {
        Pending = 10,

        Approved = 20,
    }
}
