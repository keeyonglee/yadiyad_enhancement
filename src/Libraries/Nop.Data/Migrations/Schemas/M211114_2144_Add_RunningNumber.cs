using FluentMigrator;
using Nop.Core.Domain.Documents;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/11/14 21:44:00", "M211114_2144_Add_RunningNumber")]
    public class M211114_2144_Add_RunningNumber : Migration
    {
        private readonly IMigrationManager _migrationManager;
        private readonly IRepository<RunningNumber> _runningNumberRepository;

        public M211114_2144_Add_RunningNumber(
            IMigrationManager migrationManager,
            IRepository<RunningNumber> runningNumberRepository)
        {
            _migrationManager = migrationManager;
            _runningNumberRepository = runningNumberRepository;
        }


        public M211114_2144_Add_RunningNumber(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(RunningNumber)).Exists() == false)
            {
                _migrationManager.BuildTable<RunningNumber>(Create);
            }
        }

        public override void Down()
        {
        }
    }
}
