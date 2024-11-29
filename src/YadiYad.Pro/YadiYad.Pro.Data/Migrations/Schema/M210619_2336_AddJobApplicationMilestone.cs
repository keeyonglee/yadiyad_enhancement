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
    [NopMigration("2021/06/18 23:36:00", "create tables (JobApplicationMilestone)")]

    public class M210619_2336_AddJobApplicationMilestone : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M210619_2336_AddJobApplicationMilestone(IMigrationManager migrationManager)
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
            if (Schema.Table(nameof(JobApplicationMilestone)).Exists() == false)
            {
                _migrationManager.BuildTable<JobApplicationMilestone>(Create);
            }
        }
    }
}
