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
using Nop.Core.Domain.Localization;

namespace YadiYad.Pro.Data.Mapping.Builders.JobSeeker
{
    public class ServiceLanguageProficiencyBuilder : NopEntityBuilder<ServiceLanguageProficiency>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(ServiceLanguageProficiency.ServiceProfileId)).AsInt32().ForeignKey<ServiceProfile>().Nullable()
            .WithColumn(nameof(ServiceLanguageProficiency.LanguageId)).AsInt32().ForeignKey<CommunicateLanguage>().NotNullable()
            .WithColumn(nameof(ServiceLanguageProficiency.ProficiencyLevel)).AsInt16().NotNullable()
            .WithColumn(nameof(ServiceLanguageProficiency.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(ServiceLanguageProficiency.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(ServiceLanguageProficiency.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(ServiceLanguageProficiency.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(ServiceLanguageProficiency.UpdatedOnUTC)).AsDateTime().Nullable()
            .WithColumn(nameof(ServiceLanguageProficiency.ProficiencyWrittenLevel)).AsInt16().NotNullable();

        }

        #endregion
    }
}
