using System;
using Nop.Core.Domain.Payout;
using OfficeOpenXml.Packaging.Ionic.Zlib;
using YadiYad.Pro.Core.Domain.Campaign;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Services.DTO.Payout;

namespace YadiYad.Pro.Services.Services.Campaign
{
    public class ChargesWaiverCampaign : ICampaignProcessor
    {
        public CampaignType CampaignType => CampaignType.ChargesWaiver;
        public CampaignProcessType ProcessType => CampaignProcessType.Apply;
        public void Apply<T>(int customerId, T refModel, decimal chargePercentage, Action<int> onSuccess = null)
            where T:class
        {
            if (!(refModel is PayoutRequestDTO) && !(refModel is OrderPayoutRequest))
                return;

            if (customerId <= 0)
                return;
            
            // waive percentage is actually the charge percentage from Yadiyad
            if (chargePercentage > 100 || chargePercentage < 0)
                return;

            if(refModel is PayoutRequestDTO payout)
                Apply(customerId, payout, chargePercentage, onSuccess);
            
            if(refModel is OrderPayoutRequest payout2)
                Apply(customerId, payout2, chargePercentage, onSuccess);
            
        }

        private void Apply(int customerId, PayoutRequestDTO payout, decimal wavierPercentage, Action<int> onSuccess = null)
        {
            var serviceChargeWaiver = Math.Round((payout.ServiceCharge * wavierPercentage / 100),2);
            var discountedServiceCharges = payout.ServiceCharge - serviceChargeWaiver;
            payout.Fee += serviceChargeWaiver;
            payout.ServiceCharge = discountedServiceCharges;

            if (onSuccess != null)
                onSuccess(payout.Id);
        }
        
        private void Apply(int customerId, OrderPayoutRequest payout, decimal waviverPercentage, Action<int> onSuccess = null)
        {
            var serviceChargeWaiver = Math.Round((payout.ServiceCharge ?? 0) * waviverPercentage / 100,2);
            payout.ServiceChargesWaiver = serviceChargeWaiver;
            payout.ServiceCharge = payout.ServiceCharge - payout.ServiceChargesWaiver;

            if (onSuccess != null)
                onSuccess(payout.Id);
        }
    }
}