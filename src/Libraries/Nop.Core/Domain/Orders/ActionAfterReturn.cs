namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents a return status
    /// </summary>
    public enum ActionAfterReturn
    {
        /// <summary>
        /// Received
        /// </summary>
        ReturnReceived = 10,

        /// <summary>
        /// Return raise dispute
        /// </summary>
        RaiseDispute = 20,
    }
}
