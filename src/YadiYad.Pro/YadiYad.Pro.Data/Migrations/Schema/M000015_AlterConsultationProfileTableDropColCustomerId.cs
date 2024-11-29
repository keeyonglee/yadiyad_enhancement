using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/03/20 13:12:00", "alter ConsultationProfile table drop col CustomerId")]
    public class M000015_AlterConsultationProfileTableDropColCustomerId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M000015_AlterConsultationProfileTableDropColCustomerId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if(Schema.Table("ConsultationProfile").Constraint("FK_D57DBF2D80F7F5FEC671A831A68752DBCA33F8AB").Exists())
            {
                Delete.ForeignKey("FK_D57DBF2D80F7F5FEC671A831A68752DBCA33F8AB")
                    .OnTable("ConsultationProfile");
            }
            if (Schema.Table("ConsultationProfile").Column("CustomerId").Exists())
            {
                Delete.Column("CustomerId").FromTable("ConsultationProfile");
            }
        }

        public override void Down()
        {
        }
    }
}
