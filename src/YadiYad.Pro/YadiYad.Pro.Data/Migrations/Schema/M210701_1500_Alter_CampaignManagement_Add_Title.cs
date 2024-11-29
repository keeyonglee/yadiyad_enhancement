using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Campaign;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/07/01 15:00:00", "alter CampaignManagement add title")]

    public class M210701_1500_Alter_CampaignManagement_Add_Title : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210701_1500_Alter_CampaignManagement_Add_Title(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(CampaignManagement))
               .Column(nameof(CampaignManagement.Title))
               .Exists() == false)
            {
                Create.Column(nameof(CampaignManagement.Title))
                .OnTable(nameof(CampaignManagement))
                .AsString(200)
                .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
