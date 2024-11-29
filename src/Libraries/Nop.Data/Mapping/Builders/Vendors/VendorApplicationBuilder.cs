using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Vendors;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Vendors
{
    /// <summary>
    /// Represents a vendorApplication entity builder
    /// </summary>
    public partial class VendorApplicationBuilder : NopEntityBuilder<VendorApplication>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(VendorApplication.CustomerId)).AsInt32().ForeignKey<Customer>()
                .WithColumn(nameof(VendorApplication.StoreName)).AsString(500).NotNullable()
                .WithColumn(nameof(VendorApplication.BusinessNatureCategoryId)).AsInt32().NotNullable()
                .WithColumn(nameof(VendorApplication.CategoryId)).AsInt32().Nullable()
                .WithColumn(nameof(VendorApplication.ProposedCategory)).AsString(100).Nullable()
                .WithColumn(nameof(VendorApplication.AdminComment)).AsString(2000).Nullable()
                .WithColumn(nameof(VendorApplication.IsApproved)).AsBoolean().Nullable()
                .WithColumn(nameof(VendorApplication.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(nameof(VendorApplication.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(VendorApplication.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                .WithColumn(nameof(VendorApplication.CreatedOnUtc)).AsDateTime().NotNullable()
                .WithColumn(nameof(VendorApplication.UpdatedOnUtc)).AsDateTime().Nullable();
        }

        #endregion
    }
}