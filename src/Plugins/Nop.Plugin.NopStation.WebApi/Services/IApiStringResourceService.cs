using System.Collections.Generic;
using Nop.Plugin.NopStation.WebApi.Domains;

namespace Nop.Plugin.NopStation.WebApi.Services
{
    public interface IApiStringResourceService
    {
        void DeleteApiStringResource(ApiStringResource apiStringResource);

        void InsertApiStringResource(ApiStringResource apiStringResource);

        void InsertApiStringResource(List<ApiStringResource> apiStringResources);

        void UpdateApiStringResource(ApiStringResource apiStringResource);

        ApiStringResource GetApiStringResourceById(int apiStringResourceId);

        ApiStringResource GetApiStringResourceByName(string resourceName);

        Dictionary<string, KeyValuePair<int, string>> GetAllResourceValues(int languageId);
    }
}