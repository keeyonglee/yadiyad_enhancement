using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using YadiYad.Pro.Core.Domain.Service;
using Nop.Data.Extensions;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Mapping.Builders.Service
{
    public class ServiceExpertiseBuilder : NopEntityBuilder<ServiceExpertise>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
            .WithColumn(nameof(ServiceExpertise.ServiceProfileId)).AsInt32().ForeignKey<ServiceProfile>().NotNullable()
            .WithColumn(nameof(ServiceExpertise.ExpertiseId)).AsInt32().ForeignKey<Expertise>().Nullable()
            .WithColumn(nameof(ServiceExpertise.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(ServiceExpertise.OtherExpertise)).AsString().Nullable()
            .WithColumn(nameof(ServiceExpertise.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(ServiceExpertise.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(ServiceExpertise.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(ServiceExpertise.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
