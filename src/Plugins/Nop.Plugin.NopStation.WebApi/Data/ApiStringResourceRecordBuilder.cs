using Nop.Data.Mapping.Builders;
using FluentMigrator.Builders.Create.Table;
using Nop.Plugin.NopStation.WebApi.Domains;
using FluentMigrator.SqlServer;

namespace Nop.Plugin.NopStation.WebApi.Data
{
    public class ApiStringResourceRecordBuilder : NopEntityBuilder<ApiStringResource>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ApiStringResource.ResourceName))
                .AsString().Nullable();
        }
    }
}
