using System;
using Microsoft.Extensions.ObjectPool;
using Nop.Core.Configuration;

namespace QB.Shipping.LalaMove.Infrastructure
{
    public class RateLimit : ISettings
    {
        public int RequestsPerMinute { get; set; } = 300;
        public bool EnableThrottling { get; set; } = true;
        public bool EnableWebHook { get; set; } = true;
        public DateTime BlockRequestUntil { get; set; } = DateTime.MinValue;
    }
}