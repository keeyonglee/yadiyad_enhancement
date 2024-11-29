using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Plugin.NopStation.WebApi.Models.Topics;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Web.Factories;
using Nop.Web.Models.Topics;

namespace Nop.Plugin.NopStation.WebApi.Controllers
{
    [Route("api/topic")]
    public class TopicsApiController : BaseApiController
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ITopicModelFactory _topicModelFactory;
        private readonly ITopicService _topicService;

        #endregion

        #region Ctor

        public TopicsApiController(IAclService aclService,
            ILocalizationService localizationService,
            IStoreMappingService storeMappingService,
            ITopicModelFactory topicModelFactory,
            ITopicService topicService)
        {
            _aclService = aclService;
            _localizationService = localizationService;
            _storeMappingService = storeMappingService;
            _topicModelFactory = topicModelFactory;
            _topicService = topicService;
        }

        #endregion

        #region Methods

        [HttpGet("details/{systemName}")]
        public virtual IActionResult Details(string systemName)
        {
            var model = _topicModelFactory.PrepareTopicModelBySystemName(systemName);
            if (model == null)
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.Response.Topic.FailedToLoad"));

            var response = new GenericResponseModel<TopicModel>();
            response.Data = model;
            return Ok(response);
        }

        [HttpGet("detailsbyid/{id}")]
        public virtual IActionResult DetailsById(int id)
        {
            var model = _topicModelFactory.PrepareTopicModelById(id);
            if (model == null)
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.Response.Topic.FailedToLoad"));

            var response = new GenericResponseModel<TopicModel>();
            response.Data = model;
            return Ok(response);
        }

        [HttpPost("authenticate")]
        public virtual IActionResult Authenticate([FromBody]BaseQueryModel<AuthenticateModel> queryModel)
        {
            var topic = _topicService.GetTopicBySystemName(queryModel.Data.SystemName);
            if (topic != null &&
                topic.Published &&
                //password protected?
                topic.IsPasswordProtected &&
                //store mapping
                _storeMappingService.Authorize(topic) &&
                //ACL (access control list)
                _aclService.Authorize(topic))
            {
                var response = new GenericResponseModel<TopicModel>();
                if (topic.Password != null && topic.Password.Equals(queryModel.Data.Password))
                {
                    response.Data = _topicModelFactory.PrepareTopicModelBySystemName(queryModel.Data.SystemName);
                    return Ok(response);
                }
                else
                {
                    response.ErrorList.Add(_localizationService.GetResource("Topic.WrongPassword"));
                    return BadRequest(response);
                }
            }

            return Unauthorized();
        }

        #endregion
    }
}
