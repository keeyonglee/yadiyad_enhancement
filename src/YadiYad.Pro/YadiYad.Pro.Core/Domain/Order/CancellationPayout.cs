using Nop.Core;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Core.Domain.Order
{
    public class CancellationPayout : BaseEntity
    {
        public int CancellationDefinitionId { get; set; }
        public EngagementParty EngagementParty { get; set; }
        public ChargeValueType ChargeValueType { get; set; } = ChargeValueType.Rate;
        public decimal Value { get; set; }
        public bool HasProcessingCharge { get; set; } = false;
        public EngagementChargeType RefundOnCharge { get; set; }
    }

    public enum EngagementChargeType
    {
        ProfessionalFee = 1,
        ServiceCharge = 2,
    }
}