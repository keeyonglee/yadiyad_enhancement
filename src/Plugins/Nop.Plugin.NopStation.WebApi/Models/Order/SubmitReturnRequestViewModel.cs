using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Web.Models.Order;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.NopStation.WebApi.Models.Order
{
    public class SubmitReturnRequestViewModel
    {
        [ModelBinder(BinderType = typeof(FormDataJsonBinder))]
        public SubmitReturnModel Data { get; set; }

        public List<IFormFile> Images { get; set; }
    }
}
