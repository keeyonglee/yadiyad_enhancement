using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using YadiYad.Pro.Core.Domain.Service;
using Nop.Data.Extensions;
using YadiYad.Pro.Core.Domain.Common;
using Nop.Core.Domain.Directory;

namespace YadiYad.Pro.Data.Mapping.Builders.Service
{
    public class ServiceProfileBuilder : NopEntityBuilder<ServiceProfile>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
            .WithColumn(nameof(ServiceProfile.CustomerId)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(ServiceProfile.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(ServiceProfile.DeletedFromUser)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(ServiceProfile.YearExperience)).AsInt32().NotNullable()
            .WithColumn(nameof(ServiceProfile.Company)).AsString().Nullable()
            .WithColumn(nameof(ServiceProfile.Position)).AsString().Nullable()
            .WithColumn(nameof(ServiceProfile.TenureStart)).AsDateTime().Nullable()
            .WithColumn(nameof(ServiceProfile.TenureEnd)).AsDateTime().Nullable()
            .WithColumn(nameof(ServiceProfile.AchievementAward)).AsString().Nullable()
            .WithColumn(nameof(ServiceProfile.ServiceTypeId)).AsInt32().NotNullable()
            .WithColumn(nameof(ServiceProfile.ServiceModelId)).AsInt32().NotNullable()
            .WithColumn(nameof(ServiceProfile.ServiceFee)).AsDecimal().NotNullable()
            .WithColumn(nameof(ServiceProfile.OnsiteFee)).AsDecimal().Nullable()
            .WithColumn(nameof(ServiceProfile.Availability)).AsInt32().Nullable()
            .WithColumn(nameof(ServiceProfile.CityId)).AsInt32().ForeignKey<City>().Nullable()
            .WithColumn(nameof(ServiceProfile.StateProvinceId)).AsInt32().ForeignKey<StateProvince>().Nullable()
            .WithColumn(nameof(ServiceProfile.CountryId)).AsInt32().ForeignKey<Country>().Nullable()
            .WithColumn(nameof(ServiceProfile.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(ServiceProfile.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(ServiceProfile.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(ServiceProfile.UpdatedOnUTC)).AsDateTime().Nullable()
            .WithColumn(nameof(ServiceProfile.Rating)).AsDecimal().NotNullable().WithDefaultValue(0);
        }

        #endregion
    }
}
