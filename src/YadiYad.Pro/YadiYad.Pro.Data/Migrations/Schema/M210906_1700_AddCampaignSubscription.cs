using FluentMigrator;
using Nop.Data.Migrations;
using YadiYad.Pro.Core.Domain.Campaign;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/09/06 17:00:00", "Add Campaign Subscription")]
    public class M210906_1700_AddCampaignSubscription : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210906_1700_AddCampaignSubscription(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        
        public override void Up()
        {
            _migrationManager.BuildTable<CampaignSubscription>(Create);
        }

        public override void Down()
        {
        }
    }
}