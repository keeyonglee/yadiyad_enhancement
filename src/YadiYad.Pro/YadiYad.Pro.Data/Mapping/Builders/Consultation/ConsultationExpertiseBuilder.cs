using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;

namespace YadiYad.Pro.Data.Mapping.Builders.Consultation
{
    public class ConsultationExpertiseBuilder : NopEntityBuilder<ConsultationExpertise>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
            .WithColumn(nameof(ConsultationExpertise.ConsultationProfileId)).AsInt32().ForeignKey<ConsultationProfile>().NotNullable()
            .WithColumn(nameof(ConsultationExpertise.ExpertiseId)).AsInt32().ForeignKey<Expertise>().Nullable()
            .WithColumn(nameof(ConsultationExpertise.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(ConsultationExpertise.OtherExpertise)).AsString().Nullable()
            .WithColumn(nameof(ConsultationExpertise.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(ConsultationExpertise.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(ConsultationExpertise.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(ConsultationExpertise.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
