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
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Core.Domain.Organization;
using Nop.Core.Domain.Media;

namespace YadiYad.Pro.Data.Mapping.Builders.Consultation
{
    public class ConsultationInvitationBuilder : NopEntityBuilder<ConsultationInvitation>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(ConsultationInvitation.ServiceProfileId)).AsInt32().ForeignKey<ServiceProfile>().NotNullable()
                .WithColumn(nameof(ConsultationInvitation.IndividualCustomerId)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(ConsultationInvitation.ConsultationProfileId)).AsInt32().ForeignKey<ConsultationProfile>().NotNullable()
                .WithColumn(nameof(ConsultationInvitation.OrganizationProfileId)).AsInt32().ForeignKey<OrganizationProfile>().NotNullable()
                .WithColumn(nameof(ConsultationInvitation.IsIndividualRead)).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(nameof(ConsultationInvitation.IsOrganizationRead)).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(nameof(ConsultationInvitation.ConsultationApplicationStatus)).AsInt16().NotNullable()
                .WithColumn(nameof(ConsultationInvitation.ConsultantAvailableTimeSlot)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(ConsultationInvitation.QuestionnaireAnswer)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(ConsultationInvitation.Questionnaire)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(ConsultationInvitation.Rating)).AsDouble().Nullable()
                .WithColumn(nameof(ConsultationInvitation.IsApproved)).AsBoolean().WithDefaultValue(false).Nullable()
                .WithColumn(nameof(ConsultationInvitation.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(nameof(ConsultationInvitation.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(ConsultationInvitation.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                .WithColumn(nameof(ConsultationInvitation.CreatedOnUTC)).AsDateTime().NotNullable()
                .WithColumn(nameof(ConsultationInvitation.UpdatedOnUTC)).AsDateTime().Nullable()
                .WithColumn(nameof(ConsultationInvitation.AppointmentStartDate)).AsDateTime().Nullable()
                .WithColumn(nameof(ConsultationInvitation.AppointmentEndDate)).AsDateTime().Nullable()
                .WithColumn(nameof(ConsultationInvitation.ReviewText)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(ConsultationInvitation.ReviewDateTime)).AsDateTime().Nullable()
                .WithColumn(nameof(ConsultationInvitation.RatesPerSession)).AsDecimal(11, 2).Nullable()
                .WithColumn(nameof(ConsultationInvitation.ApprovalRemarks)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(ConsultationInvitation.StatusRemarks)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(ConsultationInvitation.DeclineReasons)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(ConsultationInvitation.KnowledgenessRating)).AsDouble().Nullable()
                .WithColumn(nameof(ConsultationInvitation.ClearnessRating)).AsDouble().Nullable()
                .WithColumn(nameof(ConsultationInvitation.ProfessionalismRating)).AsDouble().Nullable()
                .WithColumn(nameof(ConsultationInvitation.RelevanceRating)).AsDouble().Nullable()
                .WithColumn(nameof(ConsultationInvitation.RespondingRating)).AsDouble().Nullable()
                .WithColumn(nameof(ConsultationInvitation.ModeratorCustomerId)).AsInt32().ForeignKey<Customer>().Nullable()
                .WithColumn(nameof(ConsultationInvitation.ModeratorReviewText)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(ConsultationInvitation.ModeratorKnowledgenessRating)).AsDouble().Nullable()
                .WithColumn(nameof(ConsultationInvitation.ModeratorClearnessRating)).AsDouble().Nullable()
                .WithColumn(nameof(ConsultationInvitation.ModeratorProfessionalismRating)).AsDouble().Nullable()
                .WithColumn(nameof(ConsultationInvitation.ModeratorRelevanceRating)).AsDouble().Nullable()
                .WithColumn(nameof(ConsultationInvitation.ModeratorRespondingRating)).AsDouble().Nullable()
                .WithColumn(nameof(ConsultationInvitation.RescheduleRemarks)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(ConsultationInvitation.CancellationDateTime)).AsDateTime().Nullable()
                .WithColumn(nameof(ConsultationInvitation.CancellationRemarks)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(ConsultationInvitation.CancellationReasonId)).AsInt32().ForeignKey<Reason>().Nullable()
                .WithColumn(nameof(ConsultationInvitation.CancellationDownloadId)).AsInt32().ForeignKey<Picture>().Nullable();


        }

        #endregion
    }
}
