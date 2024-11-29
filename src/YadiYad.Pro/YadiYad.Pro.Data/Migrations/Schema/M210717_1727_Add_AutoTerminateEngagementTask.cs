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
    [NopMigration("2021/07/17 17:27:00", "Add AutoTerminateEngagement Task")]

    public class M210717_1727_Add_AutoTerminateEngagementTask : Migration
    {
        private readonly IMigrationManager _migrationManager;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        public M210717_1727_Add_AutoTerminateEngagementTask(
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
                    Name = "Auto Terminate Engagement",
                    Seconds =int.Parse(TimeSpan.FromHours(1).TotalSeconds.ToString()),
                    Type = "YadiYad.Pro.Services.Tasks.Engagement.AutoTerminateEngagementTask, YadiYad.Pro.Services",
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
