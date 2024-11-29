using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace YadiYad.Pro.Core.Domain.DepositRequest
{

    public enum DepositRequestStatus
    {
        [Description("New")]
        New = 0,
        [Description("Paid")]
        Paid = 1,
        [Description("Reminded")]
        Reminded = 2,
        [Description("Invalid")]
        Invalid = 3,
        [Description("Overdue")]
        Overdue = 4
    }

    public enum PaymentChannel
    {
        IPay88 = 0,
        BankIn = 1
    }
}
