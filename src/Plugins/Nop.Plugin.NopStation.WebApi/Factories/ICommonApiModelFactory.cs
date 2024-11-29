using System.Collections.Generic;
using Nop.Plugin.NopStation.WebApi.Models.Common;

namespace Nop.Plugin.NopStation.WebApi.Factories
{
    public interface ICommonApiModelFactory
    {
        IList<KeyValueApi> GetStringRsources(int? languageId = null);
    }
}