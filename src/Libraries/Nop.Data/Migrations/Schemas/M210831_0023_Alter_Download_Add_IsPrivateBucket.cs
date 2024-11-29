using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/08/31 00:23:00", "Alter BankAccount Add SaltKey")]
    public class M210831_0023_Alter_Download_Add_IsPrivateBucket : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210831_0023_Alter_Download_Add_IsPrivateBucket(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Download))
               .Column(nameof(Download.IsPrivateContent))
               .Exists())
            {
                Create.Column(nameof(Download.IsPrivateContent))
                    .OnTable(nameof(Download))
                    .AsBoolean()
                    .NotNullable()
                    .WithDefaultValue(false);
            }
        }

        public override void Down()
        {
        }
    }
}
