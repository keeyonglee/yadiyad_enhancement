using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Consultation;
using Nop.Core.Domain.Directory;
using YadiYad.Pro.Core.Domain.Common;
using TimeZone = YadiYad.Pro.Core.Domain.Common.TimeZone;
using YadiYad.Pro.Core.Domain.Organization;

namespace YadiYad.Pro.Data.Mapping.Builders.Consultation
{
    public class ConsultationProfileBuilder : NopEntityBuilder<ConsultationProfile>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(ConsultationProfile.OrganizationProfileId)).AsInt32().ForeignKey<OrganizationProfile>().NotNullable().WithDefaultValue(1)
            .WithColumn(nameof(ConsultationProfile.SegmentId)).AsInt32().ForeignKey<BusinessSegment>().NotNullable()
            .WithColumn(nameof(ConsultationProfile.Topic)).AsString().NotNullable()
            .WithColumn(nameof(ConsultationProfile.Objective)).AsString(2000).NotNullable()
            .WithColumn(nameof(ConsultationProfile.YearExperience)).AsInt32().NotNullable()
            .WithColumn(nameof(ConsultationProfile.TimeZoneId)).AsInt32().ForeignKey<TimeZone>().NotNullable()
            .WithColumn(nameof(ConsultationProfile.AvailableTimeSlot)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(ConsultationProfile.Questionnaire)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(ConsultationProfile.Duration)).AsDouble().NotNullable()
            .WithColumn(nameof(ConsultationProfile.IsApproved)).AsBoolean().WithDefaultValue(false).Nullable()
            .WithColumn(nameof(ConsultationProfile.DeletedFromUser)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(ConsultationProfile.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(ConsultationProfile.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(ConsultationProfile.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(ConsultationProfile.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(ConsultationProfile.UpdatedOnUTC)).AsDateTime().Nullable()
            .WithColumn(nameof(ConsultationProfile.Remarks)).AsString(int.MaxValue).Nullable();

        }

        #endregion
    }
}
