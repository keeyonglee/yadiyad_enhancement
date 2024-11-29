using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Subscription;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/02/28 16:13:00", "add servicesubscription table")]
    public class M000012_AddServiceSubscriptionTable : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M000012_AddServiceSubscriptionTable(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<ServiceSubscription>(Create);
        }
    }
}
