using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Payout;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/08/22 18:13:00", "Alter PayoutRequest Add ProductTypeRefId")]
    public class M210822_1821_Alter_PayoutRequest_Add_ProductTypeRefId : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210822_1821_Alter_PayoutRequest_Add_ProductTypeRefId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(PayoutRequest))
               .Column(nameof(PayoutRequest.RefId))
               .Exists())
            {
                Create.Column(nameof(PayoutRequest.RefId))
                    .OnTable(nameof(PayoutRequest))
                    .AsInt32()
                    .NotNullable()
                    .WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(PayoutRequest))
               .Column(nameof(PayoutRequest.ProductTypeId))
               .Exists())
            {
                Create.Column(nameof(PayoutRequest.ProductTypeId))
                    .OnTable(nameof(PayoutRequest))
                    .AsInt32()
                    .NotNullable()
                    .WithDefaultValue(0);
            }

            Alter.Column(nameof(PayoutRequest.RefId))
                .OnTable(nameof(PayoutRequest))
                .AsInt32()
                .NotNullable();
            
            Alter.Column(nameof(PayoutRequest.ProductTypeId))
                .OnTable(nameof(PayoutRequest))
                .AsInt32()
                .NotNullable();
        }

        public override void Down()
        {
        }
    }
}
