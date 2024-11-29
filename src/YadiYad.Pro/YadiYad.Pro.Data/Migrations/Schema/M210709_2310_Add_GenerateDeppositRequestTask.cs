using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Tasks;
using Nop.Data;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Deposit;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/07/09 23:10:00", "Add GenerateDeppositRequestTask")]

    public class M210709_2310_Add_GenerateDeppositRequestTask : Migration
    {
        private readonly IMigrationManager _migrationManager;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        public M210709_2310_Add_GenerateDeppositRequestTask(
            IRepository<ScheduleTask> scheduleTaskRepository,
            IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
            _scheduleTaskRepository = scheduleTaskRepository;
        }

        public override void Up()
        {
            var tasks = new List<ScheduleTask>
            {
                new ScheduleTask
                {
                    Name = "Generate deposit request",
                    Seconds =int.Parse(TimeSpan.FromHours(1).TotalSeconds.ToString()),
                    Type = "YadiYad.Pro.Services.Tasks.Deposit.GenerateDepositRequestTask, YadiYad.Pro.Services",
                    Enabled = true,
                    StopOnError = false
                }
            };
            
            for(int i = tasks.Count -1; i>=0; i--)
            {
                var isScheduleTaskExists = _scheduleTaskRepository.Table
                    .Where(x => x.Name.ToLower() == tasks[i].Name.ToLower())
                    .Any();

                if (isScheduleTaskExists)
                {
                    tasks.RemoveAt(i);
                }
            }

            _scheduleTaskRepository.Insert(tasks);
        }

        public override void Down()
        {
        }
    }
}
