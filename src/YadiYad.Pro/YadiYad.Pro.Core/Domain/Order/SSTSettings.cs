using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Order
{
    public class SSTSettings : ISettings
    {
        public decimal SSTRate { get; set; }
        public string SSTRegNo { get; set; }
        public bool HasSST
        {
            get
            {
                var hasSST = false;

                hasSST = string.IsNullOrWhiteSpace(SSTRegNo) == false;

                return hasSST;
            }
        }
    }
}
