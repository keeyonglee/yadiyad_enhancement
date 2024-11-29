using FluentMigrator;
using Nop.Data.Migrations;
using YadiYad.Pro.Core.Domain.Individual;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/09/17 17:10:00", "Alter Bank Account add comment")]
    public class M210917_1710_Alter_BankAccount_Add_Comment : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210917_1710_Alter_BankAccount_Add_Comment(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        
        public override void Up()
        {
            if (!Schema.Table(nameof(BankAccount))
             .Column(nameof(BankAccount.Comment))
             .Exists())
            {
                Create.Column(nameof(BankAccount.Comment))
                    .OnTable(nameof(BankAccount))
                    .AsString(350)
                    .Nullable();
            }

        }

        public override void Down()
        {
        }
    }
}