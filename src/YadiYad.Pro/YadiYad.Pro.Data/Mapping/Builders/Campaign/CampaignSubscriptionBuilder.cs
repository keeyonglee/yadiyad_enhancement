using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using YadiYad.Pro.Core.Domain.Campaign;

namespace YadiYad.Pro.Data.Mapping.Builders.Campaign
{
    public class CampaignSubscriptionBuilder : NopEntityBuilder<CampaignSubscription>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CampaignSubscription.CampaignActivity)).AsInt16().NotNullable()
                .WithColumn(nameof(CampaignSubscription.CampaignId)).AsInt32().NotNullable()
                .WithColumn(nameof(CampaignSubscription.CampaignType)).AsInt16().NotNullable()
                .WithColumn(nameof(CampaignSubscription.CustomerId)).AsInt32().NotNullable()
                .WithColumn(nameof(CampaignSubscription.UsageRefId)).AsInt32().NotNullable()
                .WithColumn(nameof(CampaignSubscription.CreatedOnUTC)).AsDateTime().NotNullable()
                .WithColumn(nameof(CampaignSubscription.ActorId)).AsInt32().NotNullable();
        }
    }
}