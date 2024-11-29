using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Pro.Models.Charge
{
    public class ChargeModel : BaseNopEntityModel
    {
        public string ProductTypeName { get; set; }
        public string SubProductTypeName { get; set; }
        public int ProductTypeId { get; set; }
        public int SubProductTypeId { get; set; }
        public int SubProductTypeJobEnumId { get; set; }
        public int SubProductTypeServiceEnumId { get; set; }
        public int ValidityDays { get; set; }
        public int ValueType { get; set; }
        public string ValueTypeName { get; set; }
        public decimal Value { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool EndDateNull { get; set; }
        public decimal? MinRange { get; set; }
        public decimal? MaxRange { get; set; }
    }
}
