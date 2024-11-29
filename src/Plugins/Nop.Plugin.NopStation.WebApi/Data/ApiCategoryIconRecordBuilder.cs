using Nop.Data.Mapping.Builders;
using Nop.Plugin.NopStation.WebApi.Domains;
using FluentMigrator.Builders.Create.Table;

namespace Nop.Plugin.NopStation.WebApi.Data
{
    public class ApiCategoryIconRecordBuilder : NopEntityBuilder<ApiCategoryIcon>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
              .WithColumn(nameof(ApiCategoryIcon.CategoryId))
              .AsInt32()
              .WithColumn(nameof(ApiCategoryIcon.PictureId))
              .AsInt32()
              .WithColumn(nameof(ApiCategoryIcon.CategoryBannerId))
              .AsInt32();
        }
    }
}
