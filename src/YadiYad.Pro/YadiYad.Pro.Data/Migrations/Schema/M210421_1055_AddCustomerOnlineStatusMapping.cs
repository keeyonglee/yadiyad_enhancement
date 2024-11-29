using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/04/21 10:55:00", "add CustomerOnlineStatusMapping table")]

    public class M210421_1055_AddCustomerOnlineStatusMapping : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;
        public M210421_1055_AddCustomerOnlineStatusMapping(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            // _migrationManager.BuildTable<CustomerOnlineStatusMapping>(Create);
            if (Schema.Table(nameof(CustomerOnlineStatusMapping)).Exists() == false)
            {
                _migrationManager.BuildTable<CustomerOnlineStatusMapping>(Create);
            }
        }
    }
}
