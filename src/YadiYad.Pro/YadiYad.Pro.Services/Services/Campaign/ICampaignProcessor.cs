using System;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using YadiYad.Pro.Core.Domain.Campaign;

namespace YadiYad.Pro.Services.Services.Campaign
{
    public interface ICampaignProcessor
    {
        CampaignType CampaignType { get; }
        CampaignProcessType  ProcessType { get; }

        void Process(int customerId, decimal campaignValue, int refId = 0, Action<int> onSuccess = null)
        {
            return;
        }

        void Apply<T>(int customerId, T refModel, decimal campaignValue, Action<int> onSuccess = null) where T:class
        {
            return;
        }
    }

    public class NoOpCampaignProcessor : ICampaignProcessor
    {
        public CampaignType CampaignType { get; }
        public CampaignProcessType ProcessType { get; }
    }
}