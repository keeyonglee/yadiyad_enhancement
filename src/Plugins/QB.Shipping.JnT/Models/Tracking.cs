using System;
using System.Collections.Generic;
using System.Text;

namespace QB.Shipping.JnT.Models
{
    public class Tracking
    {
        public int QueryType { get; set; }
        public string Language { get; set; }
        public string[] QueryNumber { get; set; } 
    }
}
