using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Builders.Catalog
{
    public partial class ProductReviewPictureMappingBuilder : NopEntityBuilder<ProductReviewPictureMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductReviewPictureMapping.PictureId)).AsInt32().ForeignKey<Picture>()
                .WithColumn(nameof(ProductReviewPictureMapping.ProductReviewId)).AsInt32().ForeignKey<ProductReview>();
        }

        #endregion
    }
}
