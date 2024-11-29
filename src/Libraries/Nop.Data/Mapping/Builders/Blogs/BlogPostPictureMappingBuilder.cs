using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Media;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Builders.Blogs
{
    public partial class BlogPostPictureMappingBuilder : NopEntityBuilder<BlogPostPictureMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(BlogPostPictureMapping.PictureId)).AsInt32().ForeignKey<Picture>()
                .WithColumn(nameof(BlogPostPictureMapping.BlogPostId)).AsInt32().ForeignKey<BlogPost>();
        }

        #endregion
    }
}
