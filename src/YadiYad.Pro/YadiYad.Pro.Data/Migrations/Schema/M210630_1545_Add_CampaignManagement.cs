using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Campaign;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/30 15:45:00", "create tables (CampaignManagement)")]

    public class M210630_1545_Add_CampaignManagement : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M210630_1545_Add_CampaignManagement(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// <remarks>
        /// We use an explicit table creation order instead of an automatic one
        /// due to problems creating relationships between tables
        /// </remarks>
        /// </summary>
        public override void Up()
        {
            _migrationManager.BuildTable<CampaignManagement>(Create);
        }
    }
}
