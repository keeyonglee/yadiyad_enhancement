using System.Collections.Generic;
using Newtonsoft.Json;

namespace QB.Shipping.Borzo.Models
{
    public class BaseResponse : ErrorResponse
    {
        [JsonProperty("is_successful")]
        public bool IsSuccessful { get; set; }
    }
    
    public class ErrorResponse
    {
        [JsonProperty("errors")]
        public List<string> Errors { get; set; }
        
    }

    public class ValidResponse : BaseResponse
    {

    }

    public class ValidResponse<T> : ValidResponse
    {
        [JsonProperty("order")]
        public T Order { get; set; }
    }

    public class GetOrdersValidResponse : BaseResponse
    {

    }
    
    public class GetOrdersValidResponse<T> : GetOrdersValidResponse
    {
        [JsonProperty("orders")]
        public List<T>  Orders { get; set; }
    }

}