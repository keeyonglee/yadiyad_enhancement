using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Services.DTO.JobSeeker;
using YadiYad.Pro.Services.JobSeeker;
using YadiYad.Pro.Web.Infrastructure;
using Nop.Services.Directory;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{
    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
    public class JobSeekerController : BaseController
    {
        #region Fields

        private readonly IMapper _mapper;
        private readonly IWorkContext _workContext;
        private readonly JobSeekerProfileService _jobSeekerProfileService;
        private readonly ICountryService _countryService;


        #endregion

        #region Ctor

        public JobSeekerController(
            IMapper mapper,
            IWorkContext workContext,
            JobSeekerProfileService jobSeekerProfileService,
            ICountryService countryService)
        {
            _mapper = mapper;
            _workContext = workContext;
            _jobSeekerProfileService = jobSeekerProfileService;
            _countryService = countryService;
        }

        #endregion

        [HttpGet("")]
        public virtual IActionResult GetJobSeekerProfile()
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            var data = _jobSeekerProfileService.GetJobSeekerProfileByCustomerId(customerId);

            response.SetResponse(data);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public virtual IActionResult GetJobSeekerProfileByCustomerId(int id)
        {
            var response = new ResponseDTO();
            var data = _jobSeekerProfileService.GetJobSeekerProfileByCustomerId(id);

            response.SetResponse(data);
            return Ok(response);
        }

        [HttpPost("")]
        public IActionResult SubmitJobSeekerProfile([FromBody] JobSeekerProfileDTO dto)
        {
            
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.SetResponse(ModelState);
            }
            else
            {
                _jobSeekerProfileService.VerifyJobSeekerProfile(dto);

                var hasJobSeekerProfile = _jobSeekerProfileService
                    .HasJobSeekerProfile(_workContext.CurrentCustomer.Id);

                dto.CustomerId = _workContext.CurrentCustomer.Id;

                if (hasJobSeekerProfile == false)
                {
                    _jobSeekerProfileService.CreateJobSeekerProfile(_workContext.CurrentCustomer.Id, dto);
                }
                else
                {
                    _jobSeekerProfileService.UpdateJobSeekerProfile(_workContext.CurrentCustomer.Id, dto);
                }

                
                response.SetResponse(ResponseStatusCode.Success);
            }

            return Ok(response);
        }

    }
}
