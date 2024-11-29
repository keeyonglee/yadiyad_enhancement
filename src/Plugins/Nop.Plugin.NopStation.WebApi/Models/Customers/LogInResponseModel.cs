using Nop.Web.Models.Customer;

namespace Nop.Plugin.NopStation.WebApi.Models.Customers
{
    public class LogInResponseModel
    {
        public LogInResponseModel()
        {
            CustomerInfo = new CustomerInfoModel();
        }

        public CustomerInfoModel CustomerInfo { get; set; }

        public string Token { get; set; }
    }
}
