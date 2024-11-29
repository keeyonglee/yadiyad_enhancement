using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using QB.Shipping.JnT;
using QB.Shipping.JnT.Models;

namespace YadiYad.Pro.Tests.Services.Shipping
{
    public class FakeJntClient: HttpClient
    {
        #region Constructor
        public FakeJntClient() 
        {
        }

        public FakeJntClient(HttpMessageHandler handler)
        {
        }

        public FakeJntClient(HttpMessageHandler handler, bool disposeHandler)
        {
        }

        #endregion Constructors
        
        private const string CREATE_ORDER_ENDPOINT = "/blibli/order/createOrder";
        private ValidResponse _errorResponse = new ValidResponse();

        public new virtual Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            if (request?.Content == null)
            {
                return CreateResponseAsync(_errorResponse);
            }

            // if (request.Headers.TryGetValues("Content-Type", out IEnumerable<string> contentTypes))
            // {
            //     if (contentTypes.Any(s => s == "application/x-www-form-urlencoded"))
            //         return CreateResponseAsync(_errorResponse);
            // }
            // else
            // {
            //     return CreateResponseAsync(_errorResponse);
            // }

            if (request.Content.GetType() != typeof(FormUrlEncodedContent))
            {
                return CreateResponseAsync(_errorResponse);
            }

            // var requestData = await request.Content.ReadAsStringAsync();
            // var response = (requestData);
            //
            // // validate signature
            //
            // // validate content
            // if (request.RequestUri.AbsolutePath == CREATE_ORDER_ENDPOINT)
            // {
            //     
            //     var response = ValidateCreateOrder(request.Content.);
            // }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }

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