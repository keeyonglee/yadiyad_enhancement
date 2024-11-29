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
    [NopMigration("2021/11/14 14:30:00", "M211114_1430_Add_GenerateDepositRequestTask")]

    public class M211114_1430_Add_GenerateDepositRequestTask : Migration
    {
        private readonly IMigrationManager _migrationManager;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        public M211114_1430_Add_GenerateDepositRequestTask(
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
                    Name = "Generate Order Payout/Refund Request Task",
                    Seconds =int.Parse(TimeSpan.FromDays(1).TotalSeconds.ToString()),
                    Type = "Nop.Services.Payout.GenerateOrderPayoutRefundRequestTask, Nop.Services",
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
