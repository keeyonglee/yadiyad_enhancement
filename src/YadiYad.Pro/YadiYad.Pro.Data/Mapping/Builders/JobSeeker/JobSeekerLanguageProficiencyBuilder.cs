using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.JobSeeker;
using Nop.Core.Domain.Directory;
using YadiYad.Pro.Core.Domain.Common;
using Nop.Core.Domain.Localization;

namespace YadiYad.Pro.Data.Mapping.Builders.JobSeeker
{
    public class JobSeekerLanguageProficiencyBuilder : NopEntityBuilder<JobSeekerLanguageProficiency>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
            .WithColumn(nameof(JobSeekerLanguageProficiency.JobSeekerProfileId)).AsInt32().ForeignKey<JobSeekerProfile>().NotNullable()
            .WithColumn(nameof(JobSeekerLanguageProficiency.LanguageId)).AsInt32().ForeignKey<CommunicateLanguage>().NotNullable()
            .WithColumn(nameof(JobSeekerLanguageProficiency.ProficiencyLevel)).AsInt16().NotNullable()
            .WithColumn(nameof(JobSeekerLanguageProficiency.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(JobSeekerLanguageProficiency.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(JobSeekerLanguageProficiency.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(JobSeekerLanguageProficiency.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(JobSeekerLanguageProficiency.UpdatedOnUTC)).AsDateTime().Nullable()
            .WithColumn(nameof(JobSeekerLanguageProficiency.ProficiencyWrittenLevel)).AsInt16().NotNullable();

        }

        #endregion
    }
}
