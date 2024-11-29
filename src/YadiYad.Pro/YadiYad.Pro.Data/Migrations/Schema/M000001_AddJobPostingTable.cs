using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Job;

namespace YadiYad.Pro.Data.Migrations
{
    [NopMigration("2021/02/03 02:31:00", "create job table")]
    public class M000001_AddJobPostingTable : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M000001_AddJobPostingTable(IMigrationManager migrationManager)
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
        }
    }
}
