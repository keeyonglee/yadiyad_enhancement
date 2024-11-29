using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Vendors
{
    /// <summary>
    /// Represents a vendorApplicationSampleProductPicture entity builder
    /// </summary>
    public partial class VendorApplicationSampleProductPictureBuilder : NopEntityBuilder<VendorApplicationSampleProductPicture>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(VendorApplicationSampleProductPicture.VendorApplicationId)).AsInt32().ForeignKey<VendorApplication>()
                .WithColumn(nameof(VendorApplicationSampleProductPicture.PictureId)).AsInt32();
        }

        #endregion
    }
}