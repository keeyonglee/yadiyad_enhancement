using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Job;

namespace YadiYad.Pro.Services.Job
{
    public class JobMilestoneService
    {
        #region Fields
        private readonly IRepository<JobMilestone> _JobMilestoneRepository;

        #endregion

        #region Ctor

        public JobMilestoneService
            (IRepository<JobMilestone> JobMilestoneRepository)
        {
            _JobMilestoneRepository = JobMilestoneRepository;
        }

        #endregion


        #region Methods
        #endregion
    }
}
