using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Deposit;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/27 13:42:00", "Alter DepositRequest Add RefId, ProductTypeId")]

    public class M210627_1342_Alter_DepositRequest_AddRefIdProductTypeId : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210627_1342_Alter_DepositRequest_AddRefIdProductTypeId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(DepositRequest))
               .Column(nameof(DepositRequest.RefId))
               .Exists() == false)
            {
                Create.Column(nameof(DepositRequest.RefId))
                .OnTable(nameof(DepositRequest))
                .AsInt32()
                .NotNullable();
            }
            if (Schema.Table(nameof(DepositRequest))
               .Column(nameof(DepositRequest.ProductTypeId))
               .Exists() == false)
            {
                Create.Column(nameof(DepositRequest.ProductTypeId))
                .OnTable(nameof(DepositRequest))
                .AsInt32()
                .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
