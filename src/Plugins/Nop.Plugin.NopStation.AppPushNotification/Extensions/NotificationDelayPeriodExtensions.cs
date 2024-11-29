using System;
using Nop.Plugin.NopStation.AppPushNotification.Domains;

namespace Nop.Plugin.NopStation.AppPushNotification.Extensions
{
    public static class NotificationDelayPeriodExtensions
    {
        public static int ToMinutes(this NotificationDelayPeriod period, int value)
        {
            switch (period)
            {
                case NotificationDelayPeriod.Minutes:
                    return value;
                case NotificationDelayPeriod.Hours:
                    return value * 60;
                case NotificationDelayPeriod.Days:
                    return value * 60 * 24;
                default:
                    throw new ArgumentOutOfRangeException(nameof(period));
            }
        }
    }
}
