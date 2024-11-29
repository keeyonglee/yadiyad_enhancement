using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Organization;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Migrations.Schema.Organization
{
    [NopMigration("2021/02/07 13:02:00", "create tables (BusinessSegment, OrganizationProfile, IndividualProfile, IndividualInterestHobby)")]
    public class M000003_AddOrganizationProfileTable : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M000003_AddOrganizationProfileTable(IMigrationManager migrationManager)
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
            _migrationManager.BuildTable<BusinessSegment>(Create);
            _migrationManager.BuildTable<OrganizationProfile>(Create);
            _migrationManager.BuildTable<IndividualProfile>(Create);
            _migrationManager.BuildTable<IndividualInterestHobby>(Create);


        }
    }
}
