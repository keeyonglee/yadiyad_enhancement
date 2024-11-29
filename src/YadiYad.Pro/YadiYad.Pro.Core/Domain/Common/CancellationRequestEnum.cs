using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Common
{
    public enum CancellationRequestType
    {
        Consultation = 1,
        Job = 2,
        Service = 3,
    }

    public enum CancellationRequestBy
    {
        Buyer = 1,
        Seller
    }
}
