using Nop.Core;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Core.Domain.Order
{
    public class CancellationDefinition: BaseEntityExtension
    {
        public int EngagementId { get; set; }
        public EngagementType EngagementType { get; set; }
        public ChargeValueType ChargeValueType { get; set; }
        public PayoutCondition BasedOnCondition { get; set; } = PayoutCondition.None;
        public bool BlockCustomer { get; set; } = false;
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        private List<CancellationPayout> _payouts = new List<CancellationPayout>();

        public List<CancellationPayout> GetPayouts()
        {
            return _payouts;
        }

        public void SetPayouts(List<CancellationPayout> payouts)
        {
            _payouts = payouts;
        }
    }
}