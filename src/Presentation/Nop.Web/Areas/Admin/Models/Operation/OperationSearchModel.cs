using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Operation
{
    public class OperationSearchModel : BaseSearchModel
    {
        public string StoreName { get; set; }
    }
}
