using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Subscription
{
    public class SubscriptionDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public List<string> Features { get; set; }
        public decimal Fee { get; set; }
    }
}
