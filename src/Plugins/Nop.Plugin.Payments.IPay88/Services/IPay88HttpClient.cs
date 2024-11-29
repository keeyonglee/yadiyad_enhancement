using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Nop.Core;

namespace Nop.Plugin.Payments.IPay88.Services
{
    /// <summary>
    /// Represents the HTTP client to request IPay88 services
    /// </summary>
    public partial class IPay88HttpClient
    {
        #region Fields

        private readonly HttpClient _httpClient;
        private readonly IPay88PaymentSettings _ipay88PaymentSettings;

        #endregion

        #region Ctor

        public IPay88HttpClient(HttpClient client,
            IPay88PaymentSettings ipay88PaymentSettings)
        {
            //configure client
            client.Timeout = TimeSpan.FromSeconds(20);
            client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, $"nopCommerce-{NopVersion.CurrentVersion}");
//#if DEBUG
//            client.DefaultRequestHeaders.Add(HeaderNames.Origin, webHelper.GetStoreLocation(false));
//#else
//            client.DefaultRequestHeaders.Add(HeaderNames.Origin, webHelper.GetStoreLocation(true));
//#endif

            _httpClient = client;
            _ipay88PaymentSettings = ipay88PaymentSettings;
        }

#endregion
    }
}