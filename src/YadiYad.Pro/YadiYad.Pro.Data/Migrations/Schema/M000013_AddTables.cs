using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema.Organization
{
    [NopMigration("2021/03/10 14:40:00", "create tables (ServiceLicenseCertificate, ServiceLanguageProficiency, ServiceAcademicQualification)")]

    public class M000013_AddTables : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M000013_AddTables(IMigrationManager migrationManager)
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
            _migrationManager.BuildTable<ServiceLicenseCertificate>(Create);

            if (Schema.Table(nameof(CommunicateLanguage)).Exists() == false)
            {
                _migrationManager.BuildTable<CommunicateLanguage>(Create);
            }
            _migrationManager.BuildTable<ServiceLanguageProficiency>(Create);

            _migrationManager.BuildTable<ServiceAcademicQualification>(Create);
        }
    }
}
