using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Logging;
using Nop.Services.Tasks;
using Nop.Services.Logging;

namespace Nop.Web.Controllers
{
    //do not inherit it from BasePublicController. otherwise a lot of extra action filters will be called
    //they can create guest account(s), etc
    public partial class ScheduleTaskController : Controller
    {
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ILogger _logger;

        public ScheduleTaskController(IScheduleTaskService scheduleTaskService, ILogger logger)
        {
            _scheduleTaskService = scheduleTaskService;
            _logger = logger;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual IActionResult RunTask(string taskType)
        {
            _logger.InsertLog(LogLevel.Debug, $"SchedulerTask Invoked : {taskType}");
            var scheduleTask = _scheduleTaskService.GetTaskByType(taskType);
            if (scheduleTask == null)
                //schedule task cannot be loaded
                return NoContent();

            var task = new Task(scheduleTask);
            task.Execute();
            _logger.InsertLog(LogLevel.Debug, $"SchedulerTask Executed : {taskType}");
            
            return NoContent();
        }
    }
}