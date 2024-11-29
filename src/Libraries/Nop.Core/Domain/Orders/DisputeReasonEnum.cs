using System.ComponentModel;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents a return status
    /// </summary>
    public enum DisputeReasonEnum
    {
        InvalidReasonFromBuyer = 10,

        IncompleteDamagedReturn = 20,

        Others = 30,
    }
}
