using Amazon.S3.Encryption.Internal;
using AutoMapper;
using Moq;
using Nop.Core;
using Nop.Core.Domain.Stores;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using Nop.Services.Tests;
using Nop.Tests;
using NUnit.Framework;

namespace YadiYad.Tests
{
    [TestFixture]
    public abstract class YadiYadServiceTest : ServiceTest
    {
        private Mock<IWorkContext> _workContext;
        private Mock<IStoreContext> _storeContext;
        private Mock<IGenericAttributeService> _genericAttributeService;
        private Mock<ISettingService> _settingService;
        private DateTimeSettings _dateTimeSettings;
        protected IDateTimeHelper _dateTimeHelper;
        private Store _store;
        public ILogger _logger;
        
        public readonly IMapper _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new YadiYad.Pro.Web.Infrastructure.AutoMappingProfile());
            cfg.AddProfile(new Nop.Web.Areas.Admin.Infrastructure.Mapper.AdminMapperConfiguration());
            cfg.AddProfile(new YadiYad.Pro.Web.Infrastructure.AdminMapperConfiguration());
        }));

        public YadiYadServiceTest()
        {
            _genericAttributeService = new Mock<IGenericAttributeService>();
            _settingService = new Mock<ISettingService>();

            _workContext = new Mock<IWorkContext>();

            _logger = new NullLogger();

            _store = new Store { Id = 1 };
            _storeContext = new Mock<IStoreContext>();
            _storeContext.Setup(x => x.CurrentStore).Returns(_store);

            _dateTimeSettings = new DateTimeSettings
            {
                AllowCustomersToSetTimeZone = false,
                DefaultStoreTimeZoneId = string.Empty
            };

            _dateTimeHelper = new DateTimeHelper(_dateTimeSettings, _genericAttributeService.Object,
                _settingService.Object, _workContext.Object);
        }
    }
}