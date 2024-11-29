using System.Collections.Generic;
using Nop.Core;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Plugin.NopStation.WebApi.Services;

namespace Nop.Plugin.NopStation.WebApi.Factories
{
    public class CommonApiModelFactory : ICommonApiModelFactory
    {
        private readonly IWorkContext _workContext;
        private readonly IApiStringResourceService _apiStringResourceService;

        public CommonApiModelFactory(IWorkContext workContext,
            IApiStringResourceService apiStringResourceService)
        {
            _workContext = workContext;
            _apiStringResourceService = apiStringResourceService;
        }

        public IList<KeyValueApi> GetStringRsources(int? languageId = null)
        {
            var langId = languageId ?? _workContext.WorkingLanguage.Id;
            var model = new List<KeyValueApi>();

            var resources = _apiStringResourceService.GetAllResourceValues(langId);
            foreach (var resource in resources)
            {
                model.Add(new KeyValueApi()
                {
                    Key = resource.Key,
                    Value = resource.Value.Value
                });
            }

            return model;
        }
    }
}
