using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Attentions
{
    public class IndividualAttentionDTO
    {
        public bool HasFreelanceJobInvitesAttention { get; set; }
        public bool HasFreelanceJobAppliedAttention { get; set; }

        public bool HasFreelanceJobAttention
        {
            get
            {
                return HasFreelanceJobInvitesAttention || HasFreelanceJobAppliedAttention;
            }
        }

        public bool HasSellServiceRequestAttention { get; set; }
        public bool HasSellServiceConfirmedOrderAttention { get; set; }

        public bool HasSellServicesAttention
        {
            get
            {
                return HasSellServiceRequestAttention || HasSellServiceConfirmedOrderAttention;
            }
        }

        public bool HasBuyServiceRequestedOrderAttention { get; set; }
        public bool HasBuyServiceConfirmedOrderAttention { get; set; }

        public bool HasBuyServiceAttention
        {
            get
            {
                return HasBuyServiceRequestedOrderAttention || HasBuyServiceConfirmedOrderAttention;
            }
        }
    }
}
