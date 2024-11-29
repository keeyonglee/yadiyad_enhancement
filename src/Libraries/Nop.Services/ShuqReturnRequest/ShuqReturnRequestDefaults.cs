using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.ShuqReturnRequest
{
    public static partial class ShuqReturnRequestDefaults
    {
        public const string Pending = "Pending Seller's Decision";
        public const string InDispute = "Dispute Settlement Processing";
        public const string Return = "Request Buyer to Return Product";
        public const string Approved = "Refund Approved";
        public const string Rejected = "Refund Rejected";
    }
}
