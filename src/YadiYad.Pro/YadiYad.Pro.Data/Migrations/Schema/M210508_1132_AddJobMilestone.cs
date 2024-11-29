using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Job;

namespace YadiYad.Pro.Data.Migrations.Schema.Organization
{
    [NopMigration("2021/05/08 15:31:00", "create tables (JobMilestone)")]

    public class M210508_1132_AddServiceMilestone : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M210508_1132_AddServiceMilestone(IMigrationManager migrationManager)
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
            // _migrationManager.BuildTable<JobMilestone>(Create);
            if (Schema.Table(nameof(JobMilestone)).Exists() == false)
            {
                _migrationManager.BuildTable<JobMilestone>(Create);
            }
        }
    }
}
