using FluentMigrator;
using Nop.Core.Domain.Blogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/10/05 14:45:00", "Add BlogPostPictureMapping")]
    public class M211005_1445_Add_BlogPostPictureMapping : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211005_1445_Add_BlogPostPictureMapping(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            _migrationManager.BuildTable<BlogPostPictureMapping>(Create);
        }

        public override void Down()
        {
        }
    }
}
