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
using Nop.Core.Domain.Media;

namespace YadiYad.Pro.Data.Mapping.Builders.Individual
{
    public class BankAccountBuilder : NopEntityBuilder<BankAccount>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
        table.WithColumn(nameof(BankAccount.CustomerId)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(BankAccount.BankId)).AsInt32().ForeignKey<Bank>().Nullable()
            .WithColumn(nameof(BankAccount.AccountNumber)).AsString(200).NotNullable()
            .WithColumn(nameof(BankAccount.AccountHolderName)).AsString(200).NotNullable()
            .WithColumn(nameof(BankAccount.IsVerified)).AsBoolean().Nullable()
            .WithColumn(nameof(BankAccount.BankStatementDownloadId)).AsInt32().ForeignKey<Download>().Nullable()
            .WithColumn(nameof(BankAccount.Comment)).AsString(350).Nullable()
            .WithColumn(nameof(BankAccount.IdentityType)).AsInt32().NotNullable()
            .WithColumn(nameof(BankAccount.Identity)).AsString(200).NotNullable()
            .WithColumn(nameof(BankAccount.SaltKey)).AsString(200).NotNullable()
            .WithColumn(nameof(BankAccount.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(BankAccount.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(BankAccount.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(BankAccount.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(BankAccount.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
