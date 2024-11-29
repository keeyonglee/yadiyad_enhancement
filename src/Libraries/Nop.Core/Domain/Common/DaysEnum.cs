using System.ComponentModel;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents an order status enumeration
    /// </summary>
    public enum DaysEnum
    {
        /// <summary>
        /// 1 day
        /// </summary>
        [Description("1 day")]
        OneDay = 1,

        /// <summary>
        /// 3 day
        /// </summary>
        [Description("3 days")]
        ThreeDays = 3,

        /// <summary>
        /// 7 day
        /// </summary>
        [Description("7 days")]
        SevenDays = 7,

        /// <summary>
        /// 30 day
        /// </summary>
        [Description("30 days")]
        ThirtyDays = 30,
    }
}
