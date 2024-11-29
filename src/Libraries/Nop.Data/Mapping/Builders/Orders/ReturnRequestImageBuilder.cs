using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a vendor picture entity builder
    /// </summary>
    public partial class ReturnRequestImageBuilder : NopEntityBuilder<ReturnRequestImage>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ReturnRequestImage.DisplayOrder)).AsInt32().NotNullable()
                .WithColumn(nameof(ReturnRequestImage.PictureId)).AsInt32().NotNullable()
                .WithColumn(nameof(ReturnRequestImage.GroupReturnRequestId)).AsInt32().ForeignKey<GroupReturnRequest>();
        }

        #endregion
    }
}