using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema.Organization
{
    [NopMigration("2021/02/08 15:02:00", "create tables (Expertise,City)")]
    public class M000004_AddTables : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M000004_AddTables(IMigrationManager migrationManager)
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
            _migrationManager.BuildTable<Expertise>(Create);
            if (Schema.Table(nameof(City)).Exists() == false)
            {
                _migrationManager.BuildTable<City>(Create);
            }
        }
    }
}
