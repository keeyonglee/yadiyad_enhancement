using System.ComponentModel;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents a return status
    /// </summary>
    public enum ReturnStatusEnum
    {
        PendingCustomerToReturn = 10,

        Delivering = 20,

        PendingInspection = 30,

        ReturnReceived = 40,
    }
}
