using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema.Organization
{
    [NopMigration("2021/04/03 18:00:00", "create tables (Charge)")]

    public class M000023_AddTables : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M000023_AddTables(IMigrationManager migrationManager)
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
            // _migrationManager.BuildTable<Charge>(Create);
            if (Schema.Table(nameof(Charge)).Exists() == false)
            {
                _migrationManager.BuildTable<Charge>(Create);
            }
        }
    }
}
