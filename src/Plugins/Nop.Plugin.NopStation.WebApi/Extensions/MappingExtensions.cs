using Newtonsoft.Json;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Services.Common;
using Nop.Services.Logging;
using Nop.Services.Payments;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Nop.Plugin.NopStation.WebApi.Extensions
{
    public static class MappingExtensions
    {
        public static NameValueCollection ToNameValueCollection(this List<KeyValueApi> formValues)
        {
            var form = new NameValueCollection();
            foreach (var values in formValues)
            {
                form.Add(values.Key, values.Value);
            }
            return form;
        }

        public static void SavePaymentRequestAttribute(this IGenericAttributeService genericAttributeService, Customer customer, ProcessPaymentRequest request, int storeId)
        {
            var json = request == null ? null : JsonConvert.SerializeObject(request);
            genericAttributeService.SaveAttribute(customer, NopStationCustomerDefaults.OrderPaymentInfo, json, storeId);
        }

        public static ProcessPaymentRequest GetPaymentRequestAttribute(this IGenericAttributeService genericAttributeService, Customer customer, int storeId)
        {
            var json = genericAttributeService.GetAttribute<string>(customer, NopStationCustomerDefaults.OrderPaymentInfo, storeId);
            if (string.IsNullOrWhiteSpace(json))
                return new ProcessPaymentRequest();

            try
            {
                return JsonConvert.DeserializeObject<ProcessPaymentRequest>(json);
            }
            catch (System.Exception ex)
            {
                EngineContext.Current.Resolve<ILogger>().Error(ex.Message, ex);
                return new ProcessPaymentRequest();
            }
        }
    }
}
