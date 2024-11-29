using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using QB.Shipping.LalaMove.Models;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Threading;
using Newtonsoft.Json;
using Nop.Core.Domain.ShippingShuq;

namespace QB.Shipping.LalaMove
{
    public class LalaMoveService
    {
        #region Fields

        private readonly IHttpClientFactory _clientFactory;
        private readonly ShippingLalamoveSettings _shippingLalamoveSettings;
        #endregion

        #region Ctor

        public LalaMoveService(
           IHttpClientFactory clientFactory,
           ShippingLalamoveSettings shippingLalamoveSettings)
        {
            _clientFactory = clientFactory;
            _shippingLalamoveSettings = shippingLalamoveSettings;
        }

        #endregion

        #region Methods

        public string GetSignature(string requestContent, string secret, string epochTime, string path, bool isOrderDetails)
        {
            var rawSignature = "";
            if (isOrderDetails)
            {
                rawSignature = $"{epochTime}\r\nGET\r\n{path}/{requestContent}\r\n\r\n";
            }
            else
            {
                rawSignature = $"{epochTime}\r\nPOST\r\n{path}\r\n\r\n{requestContent}";
            }
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(rawSignature);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                var converted = BitConverter.ToString(hashmessage);
                return converted.ToLower().Replace("-", "");
            }
        }

        public string GetRawSignatureCancelOrder(string requestContent, string epochTime, string path)
        {
            return $"{epochTime}\r\nPUT\r\n{path}\r\n\r\n";
        }

        public string GetSignature(string rawSignature, string secret)
        {
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(rawSignature);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                var converted = BitConverter.ToString(hashmessage);
                return converted.ToLower().Replace("-", "");
            }
        }

        public long GetEpochTimeWithLocal()
        {
            DateTime dto = DateTime.Now;
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dto.ToUniversalTime() - epoch).TotalMilliseconds;
        }

        public double GetEpochTimeWithOffset()
        {
            DateTimeOffset dto = new DateTimeOffset(2021, 9, 21, 15, 46, 33, TimeSpan.FromHours(8));
            DateTimeOffset epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            return (dto - epoch).TotalMilliseconds;
        }

        public async Task<TOut> GetQuotationAsync<TOut>(PlaceOrderModel model, CancellationToken cancellationToken)
            where TOut : ValidResponse
        {
            return await ActAsync<TOut>(_shippingLalamoveSettings.GetQuotation, model, model.Market, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TOut> PlaceOrderAsync<TOut>(PlaceOrderModel model, CancellationToken cancellationToken)
          where TOut : ValidResponse
        {
            return await ActAsync<TOut>(_shippingLalamoveSettings.PlaceOrder, model, model.Market, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TOut> GetOrderDetailsAsync<TOut>(string orderRef, string marketCode, CancellationToken cancellationToken)
         where TOut : ValidResponse
        {
            return await ActAsyncOrderDetails<TOut>(_shippingLalamoveSettings.PlaceOrder, orderRef, marketCode, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TOut> GetCancelOrderAsync<TOut>(string orderRef, CancellationToken cancellationToken)
         where TOut : ValidResponse
        {
            return await ActAsyncCancelOrder<TOut>(_shippingLalamoveSettings.CancelOrder, orderRef, cancellationToken).ConfigureAwait(false);
        }

        //  private async Task<TOut> ActAsync<TOut>(string endpoint, object content,  bool isOrderDetails = false, CancellationToken cancellationToken)
        //where TOut : ValidResponse
        //  {
        //      //var detString = JsonConvert.SerializeObject(content);
        //      var detString = "{\"serviceType\":\"MOTORCYCLE\",\"specialRequests\":[],\"stops\":[{\"location\":{\"lat\":\"3.048593\",\"lng\":\"101.671568\"},\"addresses\":{\"ms_MY\":{\"displayString\":\"Bumi Bukit Jalil, No 2-1, Jalan Jalil 1, Lebuhraya Bukit Jalil, Sungai Besi, 57000 Kuala Lumpur, Malaysia\",\"market\":\"MY_KUL\"}}},{\"location\":{\"lat\":\"2.754873\",\"lng\":\"101.703744\"},\"addresses\":{\"ms_MY\":{\"displayString\":\"64000 Sepang, Selangor, Malaysia\",\"market\":\"MY_KUL\"}}}],\"requesterContact\":{\"name\":\"Chris Wong\",\"phone\":\"0376886555\"},\"deliveries\":[{\"toStop\":1,\"toContact\":{\"name\":\"Shen Ong\",\"phone\":\"0376886555\"},\"remarks\":\"Remarks for drop-off point (#1).\"}]}";
        //      var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
        //      httpRequest.Content = new StringContent(detString, Encoding.UTF8, "application/json");

        //      var key = _options.ApiKey;
        //      var time = GetEpochTimeWithLocal();
        //      var rawSignature = GetSignature(detString, _options.ApiSecret, time.ToString(), endpoint);

        //      var client = _clientFactory.CreateClient();
        //      client.BaseAddress = new Uri(_options.BaseUrl);
        //      client.DefaultRequestHeaders.Accept.Clear();
        //      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        //      client.DefaultRequestHeaders.Add("X-LLM-Market", "MY_KUL");
        //      client.DefaultRequestHeaders.Add("Authorization", $"hmac {key}:{time}:{rawSignature}");
        //      var responseMessage = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

        //      if (responseMessage.IsSuccessStatusCode)
        //      {
        //          var response = string.Empty;
        //          if (responseMessage?.Content != null)
        //          {
        //              response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false) ?? string.Empty;
        //          }

        //          return JsonConvert.DeserializeObject<TOut>(response);
        //      }

        //      return default;
        //  }

        private async Task<TOut> ActAsync<TOut>(string endpoint, object content, string marketCode, CancellationToken cancellationToken)
            where TOut : ValidResponse
        {
            var detString = JsonConvert.SerializeObject(content);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
            httpRequest.Content = new StringContent(detString, Encoding.UTF8, "application/json");

            var key = _shippingLalamoveSettings.ApiKey;
            var time = GetEpochTimeWithLocal();
            var rawSignature = GetSignature(detString, _shippingLalamoveSettings.ApiSecret, time.ToString(), endpoint, false);

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(_shippingLalamoveSettings.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Add("X-LLM-Market", marketCode);
            client.DefaultRequestHeaders.Add("Authorization", $"hmac {key}:{time}:{rawSignature}");
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

        private async Task<TOut> ActAsyncOrderDetails<TOut>(string endpoint, string content, string marketCode,  CancellationToken cancellationToken)
            where TOut : ValidResponse
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{endpoint}/{content}");
            //httpRequest.Content = new StringContent("", Encoding.UTF8, "application/json");

            var key = _shippingLalamoveSettings.ApiKey;
            var time = GetEpochTimeWithLocal();
            //var time = TestSpecificEpochTimeWithSetTime();
            var rawSignature = GetSignature(content, _shippingLalamoveSettings.ApiSecret, time.ToString(), endpoint, true);

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(_shippingLalamoveSettings.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Add("X-LLM-Market", marketCode);
            client.DefaultRequestHeaders.Add("Authorization", $"hmac {key}:{time}:{rawSignature}");
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

        private async Task<TOut> ActAsyncCancelOrder<TOut>(string endpoint, string content, CancellationToken cancellationToken)
           where TOut : ValidResponse
        {
            var endpointValue = String.Format(endpoint, content);
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, endpointValue);

            var key = _shippingLalamoveSettings.ApiKey;
            var time = GetEpochTimeWithLocal();

            var rawSignature = GetRawSignatureCancelOrder(content, time.ToString(), endpointValue);
            var signature = GetSignature(rawSignature, _shippingLalamoveSettings.ApiSecret);

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(_shippingLalamoveSettings.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Add("X-LLM-Market", "MY_KUL");
            client.DefaultRequestHeaders.Add("Authorization", $"hmac {key}:{time}:{signature}");
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

        #endregion
    }
}
 