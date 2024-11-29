using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Job;
using Nop.Core.Domain.Directory;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Core.Domain.Organization;
using Nop.Core.Domain.Media;

namespace YadiYad.Pro.Data.Mapping.Builders.Service
{
    public class ServiceApplicationBuilder : NopEntityBuilder<ServiceApplication>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {

            table.WithColumn(nameof(ServiceApplication.ServiceProfileId)).AsInt32().ForeignKey<ServiceProfile>().NotNullable()
                .WithColumn(nameof(ServiceApplication.CustomerId)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(ServiceApplication.StartDate)).AsDate().NotNullable()
                .WithColumn(nameof(ServiceApplication.EndDate)).AsDate().Nullable()
                .WithColumn(nameof(ServiceApplication.CancellationDateTime)).AsDateTime().Nullable()
                .WithColumn(nameof(ServiceApplication.Duration)).AsInt16().NotNullable()
                .WithColumn(nameof(ServiceApplication.Required)).AsInt16().NotNullable()
                .WithColumn(nameof(ServiceApplication.Location)).AsString().NotNullable()
                .WithColumn(nameof(ServiceApplication.ZipPostalCode)).AsString(20).NotNullable()
                .WithColumn(nameof(ServiceApplication.CityId)).AsInt32().ForeignKey<City>().Nullable()
                .WithColumn(nameof(ServiceApplication.RequesterIsRead)).AsBoolean().NotNullable()
                .WithColumn(nameof(ServiceApplication.ProviderIsRead)).AsBoolean().NotNullable()
                .WithColumn(nameof(ServiceApplication.IsEscrow)).AsBoolean().NotNullable()
                .WithColumn(nameof(ServiceApplication.Status)).AsInt32().NotNullable()
                .WithColumn(nameof(ServiceApplication.ServiceFeeAmount)).AsDecimal().NotNullable()
                .WithColumn(nameof(ServiceApplication.CommissionFeeAmount)).AsDecimal().NotNullable()
                .WithColumn(nameof(ServiceApplication.ServiceProfileServiceTypeId)).AsInt32().NotNullable()
                .WithColumn(nameof(ServiceApplication.ServiceProfileServiceModelId)).AsInt32().NotNullable()
                .WithColumn(nameof(ServiceApplication.ServiceProfileServiceFee)).AsDecimal().NotNullable()
                .WithColumn(nameof(ServiceApplication.ServiceProfileOnsiteFee)).AsDecimal().Nullable()
                .WithColumn(nameof(ServiceApplication.CancelRehire)).AsBoolean().NotNullable()
                .WithColumn(nameof(ServiceApplication.HasRehired)).AsBoolean().NotNullable()
                .WithColumn(nameof(ServiceApplication.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(nameof(ServiceApplication.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(ServiceApplication.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                .WithColumn(nameof(ServiceApplication.CreatedOnUTC)).AsDateTime().NotNullable()
                .WithColumn(nameof(ServiceApplication.UpdatedOnUTC)).AsDateTime().Nullable()
                .WithColumn(nameof(ServiceApplication.CancellationRemarks)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(ServiceApplication.CancellationReasonId)).AsInt32().ForeignKey<Reason>().Nullable()
                .WithColumn(nameof(ServiceApplication.CancellationDownloadId)).AsInt32().ForeignKey<Picture>().Nullable();

        }
        #endregion
    }
}