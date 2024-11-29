using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Migrations.Schema.Job
{
    [NopMigration("2021/02/04 00:02:00", "create tables (JobServiceCategory, InterestHobby)")]
    public class M000002_AddTables : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M000002_AddTables(IMigrationManager migrationManager)
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
            _migrationManager.BuildTable<JobServiceCategory>(Create);
            _migrationManager.BuildTable<InterestHobby>(Create);
        }
    }
}
