using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Common;
using Nop.Core.Domain.Directory;

namespace YadiYad.Pro.Data.Mapping.Builders.Individual
{
    public class IndividualInterestHobbyBuilder : NopEntityBuilder<IndividualInterestHobby>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(IndividualInterestHobby.CustomerId)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(IndividualInterestHobby.InterestHobbyId)).AsInt32().ForeignKey<InterestHobby>().Nullable()
            .WithColumn(nameof(IndividualProfile.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(IndividualProfile.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(IndividualProfile.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(IndividualProfile.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(IndividualProfile.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
