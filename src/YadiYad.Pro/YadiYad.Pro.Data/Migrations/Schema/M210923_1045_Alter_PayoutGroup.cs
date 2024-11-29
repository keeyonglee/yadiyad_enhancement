using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Payout;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    
    [NopMigration("2021/09/23 10:45:00", "M210923_1045_Alter_PayoutGroup")]
    public class M210923_1045_Alter_PayoutGroup : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210923_1045_Alter_PayoutGroup(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {

            Alter.Column(nameof(PayoutGroup.Remarks))
                .OnTable(nameof(PayoutGroup))
                .AsString()
                .Nullable();
        }

        public override void Down()
        {
        }
    }
}
