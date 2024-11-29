using FluentMigrator;
using Nop.Core.Domain.Directory;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Service;


namespace YadiYad.Pro.Data.Migrations.Schema
{

    [NopMigration("2021/04/03 17:45:00", "add CommunicateLanguage table")]
    public class M210403_1745_AddCommunicateLanguage : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;
        public M210403_1745_AddCommunicateLanguage(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            // _migrationManager.BuildTable<CommunicateLanguage>(Create);
            if (Schema.Table(nameof(CommunicateLanguage)).Exists() == false)
            {
                _migrationManager.BuildTable<CommunicateLanguage>(Create);
            }
        }
    }
}
