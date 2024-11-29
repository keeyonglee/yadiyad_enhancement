using FluentMigrator;
using Nop.Core.Domain.Blogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/10/04 15:10:00", "Alter BlogPost Add PictureId")]
    public class M211004_1510_Alter_BlogPost_Add_PictureId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211004_1510_Alter_BlogPost_Add_PictureId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(BlogPost))
                .Column(nameof(BlogPost.PictureId))
                .Exists())
            {
                Create.Column(nameof(BlogPost.PictureId))
                    .OnTable(nameof(BlogPost))
                    .AsInt32()
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
