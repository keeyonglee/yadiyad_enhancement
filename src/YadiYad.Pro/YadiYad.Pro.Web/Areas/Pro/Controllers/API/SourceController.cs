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

namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{
    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
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
        private readonly CityService _cityService;
        private readonly BankService _bankService;
        private readonly TimeZoneService _timeZoneService;
        private readonly ModeratorCancellationRequestService _cancellationReasonService;


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
            CityService cityService,
            BankService bankService,
            TimeZoneService timeZoneService,
            ModeratorCancellationRequestService cancellationReasonService)
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
            _cityService = cityService;
            _bankService = bankService;
            _timeZoneService = timeZoneService;
            _cancellationReasonService = cancellationReasonService;
        }

        #endregion

        [HttpGet("JobServiceCategoryExpertise")]
        public IActionResult GetJobServiceCategoryExpertise()
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.JobServiceExpertise);
            var result = _staticCacheManager.Get(cacheKey, () =>
            {
                var records = _jobServiceCategoryService.GetAllJobServiceCategorisExpertise();
                return records.ToList();
            });

            return Ok(new ResponseDTO(result));
        }

        [HttpPost("JobServiceCategory")]
        public IActionResult GetJobServiceCategoryList(ListFilterDTO<object> request)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.JobServiceCategory);
            var result = _staticCacheManager.Get(cacheKey, () =>
            {
                var record = _jobServiceCategoryService.GetAllJobServiceCategoris();
                return record.ToList();
            });

            var source = result.ToList();
            if (!string.IsNullOrEmpty(request.Filter))
            {
                source = source.Where(n => n.Name.ToLower().Contains(request.Filter.ToLower())).ToList();
            }
            result = new PagedList<JobServiceCategory>(source, request.Offset / request.RecordSize, request.RecordSize);

            return Ok(new ResponseDTO(result));
        }

        [HttpPost("expertise")]
        public IActionResult GetAllExpertiseList(ListFilterDTO<ExpertiseFilter> request)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.Expertise);
            var result = _staticCacheManager.Get(cacheKey, () =>
            {
                var record = _expertiseService.GetAllExpertise();
                return record.ToList();
            });

            var source = result.ToList();
            if (!string.IsNullOrEmpty(request.Filter))
            {
                source = source.Where(n => n.Name.ToLower().Contains(request.Filter.ToLower())).ToList();
            }
            if (request.AdvancedFilter != null && request.AdvancedFilter?.CategoryId != 0 && request.AdvancedFilter?.CategoryId != null)
            {
                source = source.Where(n => n.JobServiceCategoryId == request.AdvancedFilter.CategoryId).ToList();
            }
            result = new PagedList<Expertise>(source, request.Offset / request.RecordSize, request.RecordSize);

            return Ok(new ResponseDTO(result));
        }

        [HttpPost("BusinessSegment")]
        public IActionResult GetBusinessSegmentList(ListFilterDTO<object> request)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.BusinessSegment);
            var result = _staticCacheManager.Get(cacheKey, () =>
            {
                var record = _businessSegmentService.GetAllBusinessSegments();
                return record.ToList().ToList();
            });

            var source = result.ToList();
            if (!string.IsNullOrEmpty(request.Filter))
            {
                source = source.Where(n => n.Name.ToLower().Contains(request.Filter.ToLower())).ToList();
            }
            result = new PagedList<BusinessSegment>(source, request.Offset / request.RecordSize, request.RecordSize);

            return Ok(new ResponseDTO(result));
        }

        [HttpPost("InterestHobby")]
        public IActionResult GetInterestHobbyList(ListFilterDTO<object> request)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.InterestHobby);
            var result = _staticCacheManager.Get(cacheKey, () =>
            {
                var record = _interestHobbyService.GetAllInterestHobbies();
                return record.ToList();
            });

            var source = result.ToList();
            if (!string.IsNullOrEmpty(request.Filter))
            {
                source = source.Where(n => n.Name.ToLower().Contains(request.Filter.ToLower())).ToList();
            }
            result = new PagedList<InterestHobby>(source, request.Offset / request.RecordSize, request.RecordSize);

            return Ok(new ResponseDTO(result));
        }

        [HttpPost("Country")]
        public IActionResult GetCountryList(ListFilterDTO<object> request)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.Country);
            var result = _staticCacheManager.Get(cacheKey, () =>
            {
                var record = _countryService.GetAllCountriesWithPaging();
                return record.ToList();
            });

            var source = result.ToList();
            if (!string.IsNullOrEmpty(request.Filter))
            {
                source = source.Where(n => n.Name.ToLower().Contains(request.Filter.ToLower())).ToList();
            }
            result = new PagedList<Country>(source, request.Offset / request.RecordSize, request.RecordSize);

            return Ok(new ResponseDTO(result));
        }

        [HttpGet("Country/default")]
        public IActionResult GetDefaultCountry()
        {
            var defaultCountryCode = "MY";
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.CountryDefault, defaultCountryCode);
            var result = _staticCacheManager.Get(cacheKey, () =>
            {
                var record = _countryService.GetCountryByTwoLetterIsoCode(defaultCountryCode);
                return record;
            });

            return Ok(new ResponseDTO(result));
        }

        [HttpPost("StateProvince")]
        [HttpPost("StateProvince/{countryCodeC2}")]
        public IActionResult GetStateList([FromRoute] string countryCodeC2, [FromForm] ListFilterDTO<StateProvinceFilter> request)
        {
            if (string.IsNullOrWhiteSpace(countryCodeC2) == false)
            {
                var country = _countryService.GetCountryByTwoLetterIsoCode(countryCodeC2);
                if (request.AdvancedFilter == null)
                {
                    request.AdvancedFilter = new StateProvinceFilter();
                }
                request.AdvancedFilter.CountryId = country.Id;
            }

            var records = _stateProvinceService.GetStateProvincesByCountryIdWithPaging(
                request.AdvancedFilter?.CountryId ?? 0,
                request.Offset / request.RecordSize,
                request.RecordSize,
                request.Filter);

            return Ok(new ResponseDTO(records));
        }

        [HttpPost("CompanySize")]
        public IActionResult GetCompanySizeList(ListFilterDTO<object> request)
        {
            var data = Enum.GetValues(typeof(CompanySize))
                        .Cast<CompanySize>()
                        .Select(x => new SelectionDTO
                        {
                            Text = x.GetDescription(),
                            Value = (int)x
                        })
                        .ToList();

            if (!String.IsNullOrEmpty(request.Filter))
                data = data.Where(x => x.Text.ToLower().Contains(request.Filter.ToLower())).ToList();

            return Ok(new ResponseDTO(data));
        }

        [HttpPost("ExperienceYear")]
        public IActionResult GetExperienceYearList(ListFilterDTO<object> request)
        {
            var data = GetEnumAsList<ExperienceYear>(request);

            return Ok(new ResponseDTO(data));
        }

        [HttpPost("IndividualTitle")]
        public IActionResult GetIndividualTitleList(ListFilterDTO<object> request)
        {
            var data = Enum.GetValues(typeof(IndividualTitle))
                        .Cast<IndividualTitle>()
                        .Select(x => new SelectionDTO
                        {
                            Text = x.GetDescription(),
                            Value = (int)x
                        })
                        .ToList();

            if (!String.IsNullOrEmpty(request.Filter))
                data = data.Where(x => x.Text.ToLower().Contains(request.Filter.ToLower())).ToList();


            return Ok(new ResponseDTO(data));
        }

        [HttpPost("Location")]
        public IActionResult GetLocationList(ListFilterDTO<CityFilter> request)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.Location);
            var result = _staticCacheManager.Get(cacheKey, () =>
            {
                var record = _cityService.GetAllLocation();
                return record.ToList();
            });

            var source = result.ToList();
            if (!string.IsNullOrEmpty(request.Filter))
            {
                source = source.Where(n => n.DisplayName.ToLower().Contains(request.Filter.ToLower())).ToList();
            }
            if (request.AdvancedFilter != null)
            {
                if (request.AdvancedFilter?.CountryId != 0)
                {
                    source = source.Where(n => n.CountryId == request.AdvancedFilter.CountryId).ToList();
                }
                if (request.AdvancedFilter?.StateProvinceId != 0)
                {
                    source = source.Where(n => n.StateProvinceId == request.AdvancedFilter.StateProvinceId).ToList();
                }
            }
            result = new PagedList<LocationDTO>(source, request.Offset / request.RecordSize, request.RecordSize);

            return Ok(new ResponseDTO(result));
        }

        [HttpPost("AcademicQualificationType")]
        public IActionResult GetAcademicQualificationTypeList(ListFilterDTO<object> request)
        {
            var data = Enum.GetValues(typeof(AcademicQualificationType))
                        .Cast<AcademicQualificationType>()
                        .Select(x => new SelectionDTO
                        {
                            Text = x.GetDescription(),
                            Value = (int)x
                        })
                        .ToList();

            if (!String.IsNullOrEmpty(request.Filter))
                data = data.Where(x => x.Text.ToLower().Contains(request.Filter.ToLower())).ToList();

            return Ok(new ResponseDTO(data));
        }


        [HttpPost("ServiceFeeBasis")]
        public IActionResult GetServiceFeeBasisList(ListFilterDTO<object> request)
        {
            var data = Enum.GetValues(typeof(ServiceFeeBasis))
                        .Cast<ServiceFeeBasis>()
                        .Select(x => new SelectionDTO
                        {
                            Text = x.GetDescription(),
                            Value = (int)x
                        })
                        .ToList();

            if (!String.IsNullOrEmpty(request.Filter))
                data = data.Where(x => x.Text.ToLower().Contains(request.Filter.ToLower())).ToList();

            return Ok(new ResponseDTO(data));
        }

        [HttpPost("Language")]
        public IActionResult GetLanguageList(ListFilterDTO<object> request)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.Language);
            var result = _staticCacheManager.Get(cacheKey, () =>
            {
                var record = _CommunicateLanguageService.GetCommunicateLanguage();
                return record.ToList();
            });

            var source = result.ToList();
            if (!string.IsNullOrEmpty(request.Filter))
            {
                source = source.Where(n => n.Name.ToLower().Contains(request.Filter.ToLower())).ToList();
            }
            result = new PagedList<CommunicateLanguage>(source, request.Offset / request.RecordSize, request.RecordSize);

            return Ok(new ResponseDTO(result));
        }

        [HttpPost("city")]
        public IActionResult GetAllCityList(ListFilterDTO<CityFilter> request)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.City);
            var result = _staticCacheManager.Get(cacheKey, () =>
            {
                var record = _cityService.GetAllCity();
                return record.ToList();
            });

            var source = result.ToList();
            if (!string.IsNullOrEmpty(request.Filter))
            {
                source = source.Where(n => n.Name.ToLower().Contains(request.Filter.ToLower())).ToList();
            }
            if (request.AdvancedFilter != null)
            {
                if (request.AdvancedFilter?.CountryId != 0)
                {
                    source = source.Where(n => n.CountryId == request.AdvancedFilter.CountryId).ToList();
                }
                if (request.AdvancedFilter?.StateProvinceId != 0)
                {
                    source = source.Where(n => n.StateProvinceId == request.AdvancedFilter.StateProvinceId).ToList();
                }
            }
            result = new PagedList<City>(source, request.Offset / request.RecordSize, request.RecordSize);

            return Ok(new ResponseDTO(result));
        }

        [HttpPost("JobDurationPhase")]
        public IActionResult GetJobDurationPhase(ListFilterDTO<object> request)
        {
            var data = Enum.GetValues(typeof(JobDurationPhase))
                        .Cast<JobDurationPhase>()
                        .Select(x => new SelectionDTO
                        {
                            Text = x.GetDescription(),
                            Value = (int)x
                        })
                        .ToList();

            if (!String.IsNullOrEmpty(request.Filter))
                data = data.Where(x => x.Text.ToLower().Contains(request.Filter.ToLower())).ToList();

            return Ok(new ResponseDTO(data));
        }

        [HttpPost("JobPaymentPhase")]
        public IActionResult GetJobPaymentPhase(ListFilterDTO<object> request)
        {
            var data = Enum.GetValues(typeof(JobPaymentPhase))
                        .Cast<JobPaymentPhase>()
                        .Select(x => new SelectionDTO
                        {
                            Text = x.GetDescription(),
                            Value = (int)x
                        })
                        .ToList();

            if (!String.IsNullOrEmpty(request.Filter))
                data = data.Where(x => x.Text.ToLower().Contains(request.Filter.ToLower())).ToList();

            return Ok(new ResponseDTO(data));
        }

        [HttpPost("TimeZone")]
        public IActionResult GetTimeZoneList(ListFilterDTO<object> request)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.Timezone);
            var result = _staticCacheManager.Get(cacheKey, () =>
            {
                var record = _timeZoneService.GetAllTimeZones();
                return record.ToList();
            });

            var source = result.ToList();
            if (!string.IsNullOrEmpty(request.Filter))
            {
                source = source.Where(n => n.Name.ToLower().Contains(request.Filter.ToLower())).ToList();
            }
            result = new PagedList<Core.Domain.Common.TimeZone>(source, request.Offset / request.RecordSize, request.RecordSize);

            return Ok(new ResponseDTO(result));
        }

        [HttpPost("Bank")]
        public IActionResult GetAllBankList(ListFilterDTO<object> request)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.Bank);
            var result = _staticCacheManager.Get(cacheKey, () =>
            {
                var record = _bankService.GetAllBank();
                return record.ToList();
            });

            var source = result.ToList();
            if (!string.IsNullOrEmpty(request.Filter))
            {
                source = source.Where(n => n.Name.ToLower().Contains(request.Filter.ToLower())).ToList();
            }
            result = new PagedList<Bank>(source, request.Offset / request.RecordSize, request.RecordSize);

            return Ok(new ResponseDTO(result));
        }

        [HttpPost("JobModel")]
        public IActionResult GetJobModelList(ListFilterDTO<object> request)
        {
            var data = GetEnumAsList<JobModel>(request);

            return Ok(new ResponseDTO(data));
        }

        [HttpPost("JobType")]
        public IActionResult GetJobTypeList(ListFilterDTO<object> request)
        {
            var data = GetEnumAsList<JobType>(request);

            return Ok(new ResponseDTO(data));
        }

        [HttpPost("ServiceModel")]
        public IActionResult GetServiceModelList(ListFilterDTO<object> request)
        {
            var data = GetEnumAsList<ServiceModel>(request);

            return Ok(new ResponseDTO(data));
        }

        [HttpPost("ServiceType")]
        public IActionResult GetServiceTypeList(ListFilterDTO<object> request, [FromQuery] bool isService = false)
        {
            var data = new List<OptionDTO>();
            if (isService)
            {
                data = GetEnumAsList<ServiceTypeService>(request);

            }
            else
            {
                data = GetEnumAsList<ServiceType>(request);

            }

            return Ok(new ResponseDTO(data));
        }

        [HttpPost("LanguageProficiency")]
        public IActionResult GetLanguageProficiencyList(ListFilterDTO<object> request)
        {
            var data = GetEnumAsList<LanguageSpokenWrittenProficiency>(request);

            return Ok(new ResponseDTO(data));
        }

        private List<OptionDTO> GetEnumAsList<T>(ListFilterDTO<object> request)
                where T : struct, IConvertible, IComparable, IFormattable
        {
            var data = Enum.GetValues(typeof(T))
                        .Cast<T>()
                        .Select(x => new OptionDTO
                        {
                            Name = (x as Enum).GetDescription(),
                            Id = Convert.ToInt32(Enum.Parse(typeof(T), x.ToString()) as Enum)
                        })
                        .ToList();

            if (!String.IsNullOrEmpty(request.Filter))
                data = data.Where(x => x.Name.ToLower().Contains(request.Filter.ToLower())).ToList();

            return data;
        }

        [HttpPost("ConsultationReasonBuyer")]
        public IActionResult GetConsultationReasonListBuyer(ListFilterDTO<object> request)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.CancellationReason, (int)EngagementType.Consultation, (int)EngagementParty.Buyer);
            var result = _staticCacheManager.Get(cacheKey, () =>
            {
                var record = _cancellationReasonService.GetAllCancellationReason((int)EngagementType.Consultation, (int)EngagementParty.Buyer);
                return record.ToList();
            });

            var source = result.ToList();
            if (!string.IsNullOrEmpty(request.Filter))
            {
                source = source.Where(n => n.Name.ToLower().Contains(request.Filter.ToLower())).ToList();
            }
            result = new PagedList<Reason>(source, request.Offset / request.RecordSize, request.RecordSize);

            return Ok(new ResponseDTO(result));
        }

        [HttpPost("ConsultationReasonSeller")]
        public IActionResult GetConsultationReasonListSeller(ListFilterDTO<object> request)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.CancellationReason, (int)EngagementType.Consultation, (int)EngagementParty.Seller);
            var result = _staticCacheManager.Get(cacheKey, () =>
            {
                var record = _cancellationReasonService.GetAllCancellationReason((int)EngagementType.Consultation, (int)EngagementParty.Seller);
                return record.ToList();
            });

            var source = result.ToList();
            if (!string.IsNullOrEmpty(request.Filter))
            {
                source = source.Where(n => n.Name.ToLower().Contains(request.Filter.ToLower())).ToList();
            }
            result = new PagedList<Reason>(source, request.Offset / request.RecordSize, request.RecordSize);

            return Ok(new ResponseDTO(result));
        }

        [HttpPost("CancellationReason")]
        public IActionResult GetCancellationReason(ListFilterDTO<object> request, string type, string party)
        {
            int engagementType = 0;
            int engagementParty = 0;

            if (Enum.TryParse(type, true, out EngagementType resultType))
            {
                engagementType = (int)resultType;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Unable to determine Engagement Type from {nameof(type)}");
            }

            if (Enum.TryParse(party, true, out EngagementParty resultParty))
            {
                engagementParty = (int)resultParty;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Unable to determine Engagement Party from {nameof(party)}");
            }

            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.CancellationReason, engagementType, engagementParty);
            var result = _staticCacheManager.Get(cacheKey, () =>
            {
                var record = _cancellationReasonService.GetAllCancellationReason(engagementType, engagementParty);
                return record.ToList();
            });

            var source = result.ToList();
            if (!string.IsNullOrEmpty(request.Filter))
            {
                source = source.Where(n => n.Name.ToLower().Contains(request.Filter.ToLower())).ToList();
            }
            result = new PagedList<Reason>(source, request.Offset / request.RecordSize, request.RecordSize);

            return Ok(new ResponseDTO(result));
        }

        [HttpPost("IdentityType")]
        public IActionResult GetIdentityType(ListFilterDTO<object> request)
        {
            var data = Enum.GetValues(typeof(IdentityType))
                        .Cast<IdentityType>()
                        .Select(x => new SelectionDTO
                        {
                            Text = x.GetDescription(),
                            Value = (int)x
                        })
                        .ToList();

            if (!String.IsNullOrEmpty(request.Filter))
                data = data.Where(x => x.Text.ToLower().Contains(request.Filter.ToLower())).ToList();

            return Ok(new ResponseDTO(data));
        }

        [HttpPost("ServiceSearchSortBy")]
        public IActionResult GetServiceSearchSortByList(ListFilterDTO<object> request)
        {
            var data = GetEnumAsList<ServiceSearchSortBy>(request);

            return Ok(new ResponseDTO(data));
        }
    }
}
