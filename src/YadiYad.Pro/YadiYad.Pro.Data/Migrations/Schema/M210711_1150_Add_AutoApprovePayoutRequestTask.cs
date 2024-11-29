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
    [NopMigration("2021/07/11 11:50:00", "Add AutoApprovePayoutRequestTask")]

    public class M210711_1150_Add_AutoApprovePayoutRequestTask : Migration
    {
        private readonly IMigrationManager _migrationManager;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        public M210711_1150_Add_AutoApprovePayoutRequestTask(
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
                    Name = "Auto Approve Payout Request",
                    Seconds =int.Parse(TimeSpan.FromHours(1).TotalSeconds.ToString()),
                    Type = "YadiYad.Pro.Services.Tasks.Payout.AutoApprovePayoutRequestTask, YadiYad.Pro.Services",
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
