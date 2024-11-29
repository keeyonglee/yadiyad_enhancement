using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Campaign;
using Nop.Core.Domain.Customers;

namespace YadiYad.Pro.Data.Mapping.Builders.Campaign
{
    public class CampaignManagementBuilder : NopEntityBuilder<CampaignManagement>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(CampaignManagement.Channel)).AsInt16().NotNullable()
                .WithColumn(nameof(CampaignManagement.Activity)).AsInt16().NotNullable()
                .WithColumn(nameof(CampaignManagement.TransactionLimit)).AsInt16().NotNullable()
                .WithColumn(nameof(CampaignManagement.EngagementType)).AsInt16().Nullable()
                .WithColumn(nameof(CampaignManagement.Beneficiary)).AsInt16().NotNullable()
                .WithColumn(nameof(CampaignManagement.Type)).AsInt16().NotNullable()
                .WithColumn(nameof(CampaignManagement.Value1)).AsDecimal().NotNullable()
                .WithColumn(nameof(CampaignManagement.Value2)).AsDecimal().NotNullable()
                .WithColumn(nameof(CampaignManagement.From)).AsDateTime().NotNullable()
                .WithColumn(nameof(CampaignManagement.Until)).AsDateTime().Nullable()
                .WithColumn(nameof(CampaignManagement.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(nameof(CampaignManagement.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(CampaignManagement.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                .WithColumn(nameof(CampaignManagement.CreatedOnUTC)).AsDateTime().NotNullable()
                .WithColumn(nameof(CampaignManagement.UpdatedOnUTC)).AsDateTime().Nullable()
                .WithColumn(nameof(CampaignManagement.Title)).AsString(200).NotNullable();







        }
    }
}
