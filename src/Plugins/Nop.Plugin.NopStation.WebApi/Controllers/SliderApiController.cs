using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.NopStation.WebApi.Factories;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Plugin.NopStation.WebApi.Models.Sliders;
using Nop.Services.Stores;

namespace Nop.Plugin.NopStation.WebApi.Controllers
{
    [Route("api/slider")]
    public class SliderApiController : BaseApiController
    {
        #region Field

        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ISliderModelFactory _sliderModelFactory;

        #endregion

        #region Ctor

        public SliderApiController(IStoreService storeService,
            IWorkContext workContext,
            ISliderModelFactory sliderModelFactory)
        {
            _storeService = storeService;
            _workContext = workContext;
            _sliderModelFactory = sliderModelFactory;
        }

        #endregion

        #region Action Method

        [HttpGet("homepageslider")]
        public IActionResult HomePageSlider()
        {
            var response = new GenericResponseModel<HomePageSliderModel>();
            response.Data = _sliderModelFactory.PrepareHomePageSliderModel();
            return Ok(response);
        }

        #endregion
    }
}
