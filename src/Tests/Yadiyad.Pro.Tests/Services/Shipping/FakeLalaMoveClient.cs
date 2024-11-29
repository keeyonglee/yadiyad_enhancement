using Newtonsoft.Json;
using QB.Shipping.JnT.Models;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YadiYad.Pro.Tests.Services.Shipping
{
    public class FakeLalaMoveClient : HttpClient
    {
        #region Constructor

        public FakeLalaMoveClient()
        {

        }

        #endregion

        private const string GET_QUOTATION_ENDPOINT = "/v2/quotations";
        private ValidResponse _errorResponse = new ValidResponse();

        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            return SendAsync(request);
        }

        private async Task<HttpResponseMessage> CreateResponseAsync(object content)
        {
            var responseContent = JsonConvert.SerializeObject(content);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(responseContent, Encoding.UTF8, "application/json");
            return await Task.FromResult(response);
        }
    }
}
