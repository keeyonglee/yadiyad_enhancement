using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Threading;
using QB.Shipping.JnT.Models;
using System.IO;
using Nop.Core.Domain.ShippingShuq;

namespace QB.Shipping.JnT
{
    public class JntService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ShippingJntSettings _shippingJntSettings;

        public JntService(
            IHttpClientFactory clientFactory, 
            ShippingJntSettings shippingJntSettings)
        {
            _clientFactory = clientFactory;
            _shippingJntSettings = shippingJntSettings;
        }
        
        public string GetSignature(string requestContent, string key)
        {
            var signature = $"{requestContent}{key}";
            var md5Hash =  GetMd5Hash(signature);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(md5Hash));
        }

        public string GetSignatureConsignmentNote(string requestContent)
        {
            var signature = $"{requestContent}";
            return GetMd5Hash(signature);
        }

        public async Task<TOut> CreateConsignmentNoteAsync<TOut>(string airwaybill, CancellationToken cancellationToken)
         where TOut : ValidResponse
        {
            var consignmentInterface = new ConsignmentNoteInterface
            {
                CustomerAccount = _shippingJntSettings.Username,
                CustomerPassword = _shippingJntSettings.ApiKey,
                CustomerId = _shippingJntSettings.CustomerCode,
                Airwaybill = airwaybill
            };
            var consignmentRequest = new ConsignmentNoteRequest
            {
                MessageType = "1",
                Data = consignmentInterface
            };

            return await ActAsyncConsignmentNote<TOut>(_shippingJntSettings.ConsignmentNote, consignmentRequest, airwaybill, cancellationToken).ConfigureAwait(false);
        }

        public async Task<byte[]> CreateConsignmentPdfAsync(string airwaybill, CancellationToken cancellationToken)
        {
            var consignmentInterface = new ConsignmentNoteInterface
            {
                CustomerAccount = _shippingJntSettings.Username,
                CustomerPassword = _shippingJntSettings.ApiKey,
                CustomerId = _shippingJntSettings.CustomerCode,
                Airwaybill = airwaybill
            };
            var consignmentRequest = new ConsignmentNoteRequest
            {
                MessageType = "1",
                Data = consignmentInterface
            };

            return await ActAsyncConsignmentNotePdf(_shippingJntSettings.ConsignmentNote, consignmentRequest, airwaybill, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TOut> TrackingAsync<TOut>(Tracking tracking, CancellationToken cancellationToken)
            where TOut : TrackingValidResponse
        {
            var trackingLogistics = new LogisticsInterface
            {
                QueryType = tracking.QueryType,
                Language = tracking.Language,
                QueryNumber= tracking.QueryNumber
            };

            return await TrackAsync<TOut>(_shippingJntSettings.Tracking, trackingLogistics, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<TOut> CreateOrderAsync<TOut>(OrderRequestDetails shipment, CancellationToken cancellationToken)
            where TOut : ValidResponse
        {
            var requestDetails = new OrderRequest();
            requestDetails.Details = shipment;

            return await ActAsync<TOut>(_shippingJntSettings.CreateOrder, requestDetails, cancellationToken).ConfigureAwait(false);
        }

        private async Task<TOut> ActAsyncConsignmentNote<TOut>(string endpoint, ConsignmentNoteRequest consignmentContent, string awbill, CancellationToken cancellationToken)
         where TOut : ValidResponse
        {
            var detString = JsonConvert.SerializeObject(consignmentContent.Data);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
            httpRequest.Content = GetContentConsignmentNote(detString, awbill);

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(_shippingJntSettings.BaseUrl);

            var responseMessage = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                var response = string.Empty;
                if (responseMessage?.Content != null)
                {
                    response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false) ?? string.Empty;
                }

                return JsonConvert.DeserializeObject<TOut>(response);
            }

            return default;
        }

        private async Task<byte[]> ActAsyncConsignmentNotePdf(string endpoint, ConsignmentNoteRequest consignmentContent, string awbill, CancellationToken cancellationToken)
        {
            var detString = JsonConvert.SerializeObject(consignmentContent.Data);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
            httpRequest.Content = GetContentConsignmentNote(detString, awbill);

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(_shippingJntSettings.BaseUrl);

            var responseMessage = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                return responseMessage.Content
                    .ReadAsByteArrayAsync()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }

            return default;
        }

        private async Task<TOut> TrackAsync<TOut>(string endpoint, object content, CancellationToken cancellationToken)
            where TOut : TrackingValidResponse
        {
            var detString = JsonConvert.SerializeObject(content);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
            httpRequest.Content = GetContent(detString, endpoint);

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(_shippingJntSettings.BaseUrl);

            var responseMessage = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                var response = string.Empty;
                if (responseMessage?.Content != null)
                {
                    response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false) ?? string.Empty;
                }

                return JsonConvert.DeserializeObject<TOut>(response);
            }

            return default;
        }

        private async Task<TOut> ActAsync<TOut>(string endpoint, object content, CancellationToken cancellationToken)
         where TOut : ValidResponse
        {
            var detString = JsonConvert.SerializeObject(content);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
            httpRequest.Content = GetContent(detString, endpoint);

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(_shippingJntSettings.BaseUrl);

            var responseMessage = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                var response = string.Empty;
                if (responseMessage?.Content != null)
                {
                    response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false) ?? string.Empty;
                }

                return JsonConvert.DeserializeObject<TOut>(response);
            }

            return default;
        }

        private FormUrlEncodedContent GetContentConsignmentNote(string requestContent, string awbill)
        {
            var requestBody = new Dictionary<string, string>
                {
                    { "logistics_interface", requestContent },
                    { "msg_type", "1"},
                    { "data_digest", GetSignatureConsignmentNote(awbill) + "|"}
                };

            return new FormUrlEncodedContent(requestBody);
        }

        private FormUrlEncodedContent GetContent(string requestContent, string endPoint)
        {
            var key = "";
            var requestBody = new Dictionary<string, string>();
            if (endPoint.Contains("order"))
            {
                key = _shippingJntSettings.OrderKey;
                requestBody = new Dictionary<string, string>
                {
                    { "data_sign", GetSignature(requestContent, key) },
                    { "data_param", requestContent }
                };
            }
            else if (endPoint.Contains("track"))
            {
                key = _shippingJntSettings.TrackingKey;
                requestBody = new Dictionary<string, string>
                {
                    { "logistics_interface", requestContent },
                    { "data_digest", GetSignature(requestContent, key) },
                    { "msg_type", "TRACK"},
                    { "eccompanyid", _shippingJntSettings.ECompanyId}
                };
            }
            else if (endPoint.Contains("print"))
            {
                requestBody = new Dictionary<string, string>
                {
                    { "logistics_interface", requestContent },
                    { "msg_type", "1"},
                    { "data_digest", GetSignatureConsignmentNote(requestContent)}
                };
            }

            return new FormUrlEncodedContent(requestBody);
        }

        private string GetMd5Hash(string content)
        {
            using var md5 = MD5.Create();
            var hashArray = md5.ComputeHash(Encoding.UTF8.GetBytes(content));
            var hash = BitConverter.ToString(hashArray);
            return hash.Replace("-", string.Empty).ToLower();
        }
    }
}