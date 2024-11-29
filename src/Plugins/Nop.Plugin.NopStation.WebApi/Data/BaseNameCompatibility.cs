using System;
using System.Collections.Generic;
using Nop.Data.Mapping;
using Nop.Plugin.NopStation.WebApi.Domains;

namespace Nop.Plugin.NopStation.WebApi.Data
{
    public class BaseNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(ApiCategoryIcon), "NS_ApiCategoryIcon" },
            { typeof(ApiDevice), "NS_ApiDevice" },
            { typeof(ApiSlider), "NS_ApiSlider" },
            { typeof(ApiStringResource), "NS_ApiStringResource" }
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>
        {
        };
    }
}
