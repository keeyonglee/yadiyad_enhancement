using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Core.Domain.ShippingShuq;
using QB.Shipping.Borzo.Models;

namespace QB.Shipping.Borzo
{
    public class BorzoService
    {
        #region Fields

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ShippingBorzoSettings _shippingBorzoSettings;

        #endregion

        #region Ctor

        public BorzoService(IHttpClientFactory httpClientFactory,
            ShippingBorzoSettings shippingBorzoSettings)
        {
            _httpClientFactory = httpClientFactory;
            _shippingBorzoSettings = shippingBorzoSettings;
        }

        #endregion

        #region Utilities

        private async Task<string> GetRawBodyStringAsync(HttpRequest request, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            
            request.EnableBuffering();
            request.Body.Position = 0;
            
            using StreamReader reader = new StreamReader(request.Body, encoding);
            return await reader.ReadToEndAsync();
        }

        #endregion

        #region Methods

        public async Task<TOut> GetAsync<TOut>(string endpoint, object requestModel, CancellationToken cancellationToken)
            where TOut : ValidResponse
        {
            if (endpoint.ToLower().Contains("client"))
            {
                return await ActGetClientAsync<TOut>(endpoint, cancellationToken).ConfigureAwait(false);
            }
            else if (endpoint.ToLower().Contains("calculate"))
            {
                return await ActGetQuotationAsync<TOut>(endpoint, requestModel, cancellationToken).ConfigureAwait(false);
            }
            else if (endpoint.ToLower().Contains("create"))
            {
                return await ActPlaceOrderAsync<TOut>(endpoint, requestModel, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                throw new ArgumentException($"{endpoint} is not valid endpoint");
            }
        }

        public async Task<TOut> GetOrdersAsync<TOut>(string endpoint, int orderId, CancellationToken cancellationToken)
            where TOut : GetOrdersValidResponse
        {
            return await ActGetOrdersAsync<TOut>(endpoint, orderId, cancellationToken).ConfigureAwait(false);
        }
        
        private async Task<TOut> ActGetClientAsync<TOut>(string endpoint, CancellationToken cancellationToken)
            where TOut : ValidResponse
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_shippingBorzoSettings.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Add("X-DV-Auth-Token", _shippingBorzoSettings.ApiSecret);
            var responseMessage = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

            var response = string.Empty;
            if (responseMessage?.Content != null)
            {
                response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false) ?? string.Empty;
                return JsonConvert.DeserializeObject<TOut>(response);
            }
            else
            {
                return default;
            }
        }
        
        private async Task<TOut> ActGetOrdersAsync<TOut>(string endpoint, int orderId, CancellationToken cancellationToken)
            where TOut : GetOrdersValidResponse
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{endpoint}?order_id={orderId}");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_shippingBorzoSettings.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Add("X-DV-Auth-Token", _shippingBorzoSettings.ApiSecret);
            var responseMessage = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

            var response = string.Empty;
            if (responseMessage?.Content != null)
            {
                response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false) ?? string.Empty;
                return JsonConvert.DeserializeObject<TOut>(response);
            }
            else
            {
                return default;
            }
        }
        
        private async Task<TOut> ActGetQuotationAsync<TOut>(string endpoint, object content, CancellationToken cancellationToken)
            where TOut : ValidResponse
        {
            var detString = JsonConvert.SerializeObject(content);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
            httpRequest.Content = new StringContent(detString, Encoding.UTF8, "application/json");
            
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_shippingBorzoSettings.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Add("X-DV-Auth-Token", _shippingBorzoSettings.ApiSecret);
            var responseMessage = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

            var response = string.Empty;
            if (responseMessage?.Content != null)
            {
                response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false) ?? string.Empty;
                return JsonConvert.DeserializeObject<TOut>(response);
            }
            else
            {
                return default;
            }
        }
        
        private async Task<TOut> ActPlaceOrderAsync<TOut>(string endpoint, object content, CancellationToken cancellationToken)
            where TOut : ValidResponse
        {
            var detString = JsonConvert.SerializeObject(content);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
            httpRequest.Content = new StringContent(detString, Encoding.UTF8, "application/json");
            
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_shippingBorzoSettings.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Add("X-DV-Auth-Token", _shippingBorzoSettings.ApiSecret);
            var responseMessage = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

            var response = string.Empty;
            if (responseMessage?.Content != null)
            {
                response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false) ?? string.Empty;
                return JsonConvert.DeserializeObject<TOut>(response);
            }
            else
            {
                return default;
            }
        }
        
        

        public Tuple<string, string> GetCalculatedSignature(HttpRequest request)
        {
            var requestRawBody = GetRawBodyStringAsync(request)
                .GetAwaiter()
                .GetResult();
            
            var encoding = new UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(_shippingBorzoSettings.CallbackSecret);
            byte[] messageBytes = encoding.GetBytes(requestRawBody);
            using var hmacsha256 = new HMACSHA256(keyByte);
            byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
            var converted = BitConverter.ToString(hashmessage);
            return Tuple.Create(requestRawBody, converted.ToLower().Replace("-", ""));
        }

        #endregion
        
    }
}