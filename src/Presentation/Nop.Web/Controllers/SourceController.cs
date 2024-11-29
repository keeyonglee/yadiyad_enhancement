using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Organization;
using YadiYad.Pro.Services.Job;
using YadiYad.Pro.Services.Common;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Web.Infrastructure;
using Nop.Services.Directory;
using YadiYad.Pro.Core.Domain.Service;
using Nop.Services.Localization;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Web.DTO.Filters;
using YadiYad.Pro.Services.Services.Common;
using Nop.Services.Security;
using YadiYad.Pro.Web.Filters;
using YadiYad.Pro.Services.Services.Moderator;
using Nop.Services.Caching;
using Nop.Core.Caching;
using YadiYad.Pro.Core.Infrastructure.Cache;
using Nop.Core.Domain.Directory;
using YadiYad.Pro.Services.DTO.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Factories;

namespace Nop.Web.Controllers
{
    [CamelCaseResponseFormatter]
    [Route("api/shuq/[controller]")]
    public class SourceController : BaseController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly JobServiceCategoryService _jobServiceCategoryService;
        private readonly BusinessSegmentService _businessSegmentService;
        private readonly InterestHobbyService _interestHobbyService;
        private readonly CountryService _countryService;
        private readonly StateProvinceService _stateProvinceService;
        private readonly CommunicateLanguageService _CommunicateLanguageService;
        private readonly ExpertiseService _expertiseService;
        private readonly BankService _bankService;
        private readonly TimeZoneService _timeZoneService;
        private readonly ModeratorCancellationRequestService _cancellationReasonService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;


        #endregion

        #region Ctor

        public SourceController(
            IWorkContext workContext,
            ICacheKeyService cacheKeyService,
            IStaticCacheManager staticCacheManager,
            JobServiceCategoryService jobServiceCategoryService,
            BusinessSegmentService businessSegmentService,
            InterestHobbyService interestHobbyService,
            CountryService countryService,
            StateProvinceService stateProvinceService,
            CommunicateLanguageService communicateLanguageService,
            ExpertiseService expertiseService,
            BankService bankService,
            TimeZoneService timeZoneService,
            ModeratorCancellationRequestService cancellationReasonService,
            IBaseAdminModelFactory baseAdminModelFactory)
        {
            _workContext = workContext;
            _cacheKeyService = cacheKeyService;
            _staticCacheManager = staticCacheManager;
            _jobServiceCategoryService = jobServiceCategoryService;
            _businessSegmentService = businessSegmentService;
            _interestHobbyService = interestHobbyService;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _CommunicateLanguageService = communicateLanguageService;
            _expertiseService = expertiseService;
            _bankService = bankService;
            _timeZoneService = timeZoneService;
            _cancellationReasonService = cancellationReasonService;
            _baseAdminModelFactory = baseAdminModelFactory;

        }

        #endregion

        [HttpPost("EatCategory")]
        public IActionResult GetEatCategoryList(ListFilterDTO<object> request)
        {

            var result = new List<SelectListItem>();
            _baseAdminModelFactory.PrepareEatCategories(result);

            var source = result.ToList();
            if (!string.IsNullOrEmpty(request.Filter))
            {
                source = source.Where(n => n.Text.ToLower().Contains(request.Filter.ToLower())).ToList();
            }
            result = new PagedList<SelectListItem>(source, request.Offset / request.RecordSize, request.RecordSize);

            return Ok(new ResponseDTO(result));
        }

        [HttpPost("MartCategory")]
        public IActionResult GetMartCategoryList(ListFilterDTO<object> request)
        {
            var result = new List<SelectListItem>();
            _baseAdminModelFactory.PrepareMartCategories(result);

            var source = result.ToList();
            if (!string.IsNullOrEmpty(request.Filter))
            {
                source = source.Where(n => n.Text.ToLower().Contains(request.Filter.ToLower())).ToList();
            }
            result = new PagedList<SelectListItem>(source, request.Offset / request.RecordSize, request.RecordSize);

            return Ok(new ResponseDTO(result));
        }

    }
}
