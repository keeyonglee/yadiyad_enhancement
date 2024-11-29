using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/09/28 11:21:00", "Add Seller Dispute Picture")]
    public class M210928_1121_Add_SellerDisputePicture : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210928_1121_Add_SellerDisputePicture(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            _migrationManager.BuildTable<SellerDisputePicture>(Create);
        }

        public override void Down()
        {
        }
    }
}
