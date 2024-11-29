using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Service;
using Nop.Core.Domain.Directory;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Mapping.Builders.Service
{
    public class ServiceAcademicQualificationBuilder : NopEntityBuilder<ServiceAcademicQualification>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(ServiceAcademicQualification.ServiceProfileId)).AsInt32().ForeignKey<ServiceProfile>().Nullable()
            .WithColumn(nameof(ServiceAcademicQualification.AcademicQualificationType)).AsInt16().NotNullable()
            .WithColumn(nameof(ServiceAcademicQualification.AcademicQualificationName)).AsString().NotNullable()
            .WithColumn(nameof(ServiceAcademicQualification.AcademicInstitution)).AsString().NotNullable()
            .WithColumn(nameof(ServiceAcademicQualification.IsHighest)).AsBoolean().NotNullable()
            .WithColumn(nameof(ServiceAcademicQualification.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(ServiceAcademicQualification.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(ServiceAcademicQualification.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(ServiceAcademicQualification.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(ServiceAcademicQualification.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
