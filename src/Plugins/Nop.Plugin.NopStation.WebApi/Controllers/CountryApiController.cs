using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Web.Factories;
using Nop.Web.Models.Directory;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.NopStation.WebApi.Controllers
{
    [Route("api/country")]
    public class CountryApiController : BaseApiController
    {
        #region Fields

        private readonly ICountryModelFactory _countryModelFactory;

        #endregion

        #region Ctor

        public CountryApiController(ICountryModelFactory countryModelFactory)
        {
            _countryModelFactory = countryModelFactory;
        }

        #endregion

        #region States / provinces

        [HttpGet("getstatesbycountryid/{countryId}/{addSelectStateItem?}")]
        public IActionResult GetStatesByCountryId(string countryId, bool addSelectStateItem = false)
        {
            var response = new GenericResponseModel<List<StateProvinceModel>>();
            response.Data = _countryModelFactory.GetStatesByCountryId(countryId, addSelectStateItem).ToList();
            return Ok(response);
        }

        #endregion
    }
}
