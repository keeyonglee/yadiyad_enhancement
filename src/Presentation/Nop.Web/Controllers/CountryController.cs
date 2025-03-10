﻿using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers
{
    public partial class CountryController : BasePublicController
	{
        #region Fields

        private readonly ICountryModelFactory _countryModelFactory;
        
        #endregion
        
        #region Ctor

        public CountryController(ICountryModelFactory countryModelFactory)
		{
            _countryModelFactory = countryModelFactory;
		}
        
        #endregion
        
        #region States / provinces

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        [HttpGet, ActionName("GetStatesByCountryId")]
        public virtual IActionResult GetStatesByCountryId(string countryId, bool addSelectStateItem)
        {
            var model = _countryModelFactory.GetStatesByCountryId(countryId, addSelectStateItem);
            return Json(model);
        }

        #endregion

        #region City

        [CheckAccessPublicStore(true)]
        [HttpGet, ActionName("GetCitiesByStateId")]
        public virtual IActionResult GetCitiesByStateId(string stateId, bool addSelectStateItem)
        {
            var model = _countryModelFactory.GetCitiesByStateId(stateId, addSelectStateItem);
            return Json(model);
        }

        #endregion
    }
}