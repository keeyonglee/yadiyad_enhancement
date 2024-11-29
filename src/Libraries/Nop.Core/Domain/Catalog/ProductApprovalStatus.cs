using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Catalog
{
    public enum ProductApprovalStatus
    {
        Rejected = -1,
        Draft = 4,
        Approved = 1,
        Unpublished = 2,
        PendingApproval = 3
    }
}
