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
    [NopMigration("2021/03/22 01:00:00", "create tables (ProOrder, ProOrderItem)")]

    public class M000019_AddTables : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M000019_AddTables(IMigrationManager migrationManager)
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
            _migrationManager.BuildTable<ProOrder>(Create);

            if (Schema.Table(nameof(ProInvoice)).Exists() == false)
            {
                _migrationManager.BuildTable<ProInvoice>(Create);
            }
            _migrationManager.BuildTable<ProOrderItem>(Create);
        }
    }
}
