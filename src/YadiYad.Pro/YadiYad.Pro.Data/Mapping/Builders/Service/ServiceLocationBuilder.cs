//using FluentMigrator.Builders.Create.Table;
//using Nop.Core.Domain.Customers;
//using Nop.Data.Mapping.Builders;
//using YadiYad.Pro.Core.Domain.Service;
//using Nop.Data.Extensions;
//using Nop.Core.Domain.Directory;
//using YadiYad.Pro.Core.Domain.Common;

//namespace YadiYad.Pro.Data.Mapping.Builders.Service
//{
//    public class ServiceLocationBuilder : NopEntityBuilder<ServiceLocation>
//    {
//        #region Methods

//        public override void MapEntity(CreateTableExpressionBuilder table)
//        {
//            table
//            .WithColumn(nameof(ServiceLocation.ServiceProfileId)).AsInt32().ForeignKey<ServiceProfile>().NotNullable()
//            .WithColumn(nameof(ServiceLocation.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
//            .WithColumn(nameof(ServiceLocation.CityId)).AsInt32().ForeignKey<City>().Nullable()
//            .WithColumn(nameof(ServiceLocation.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
//            .WithColumn(nameof(ServiceLocation.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
//            .WithColumn(nameof(ServiceLocation.CreatedOnUTC)).AsDateTime().NotNullable()
//            .WithColumn(nameof(ServiceLocation.UpdatedOnUTC)).AsDateTime().Nullable();
//        }

//        #endregion
//    }
//}
