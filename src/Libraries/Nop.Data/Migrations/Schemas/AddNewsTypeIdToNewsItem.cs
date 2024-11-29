using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator;
using Nop.Core.Domain.News;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/01/31 20:36:08:9037708")]
    public class AddNewsTypeIdToNewsItem : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            if (Schema.Table(nameof(NewsItem)).Column(nameof(NewsItem.NewsTypeId)).Exists() == false)
            {
                //Create
                //.Column(nameof(NewsItem.NewsTypeId))
                //.OnTable(NameCompatibilityManager.GetTableName(typeof(NewsItem)))
                //.AsInt32()
                //.WithDefaultValue(0);
            }
        }

        #endregion
    }
}
