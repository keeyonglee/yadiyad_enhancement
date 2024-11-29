using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace YadiYad.Pro.Web.DTO.Base
{
    public class ResponseDTO
    {
        public StatusInfo Status { get; private set; }
        public object Data { get; private set; }

        public ResponseDTO()
        {
            Status = new StatusInfo();
            SetResponse(ResponseStatusCode.Success);
        }

        public ResponseDTO(object data)
        {
            SetResponse(ResponseStatusCode.Success);
            Data = data;
        }

        public ResponseDTO(params AppResponse[] responses)
        {
            SetResponse(ResponseStatusCode.Warning);

            var objs = new List<object>();

            foreach (var response in responses)
            {
                objs.Add(
                    new
                    {
                        code = (int)response,
                        message = response.GetDescription()
                    }
                    );
            }

            Data = objs;
        }

        public ResponseDTO(ResponseStatusCode statusCode, object data)
        {
            SetResponse(statusCode, null);
            Data = data;
        }

        public void SetResponse(object data)
        {
            SetResponse(ResponseStatusCode.Success);
            Data = data;
        }

        public void SetResponse(ResponseStatusCode statusCode, string statusMsg = null)
        {
            StatusInfo status = new StatusInfo
            {
                Code = (int)statusCode
            };

            switch (statusCode)
            {
                case ResponseStatusCode.Success:
                    status.Message = "Success";
                    break;
                case ResponseStatusCode.Warning:
                    status.Message = "Warning";
                    break;
            }

            if(string.IsNullOrWhiteSpace(statusMsg) == false)
            {
                status.Message = statusMsg;
            }

            Status = status;
        }

        public void SetResponse(ModelStateDictionary modelStateDictionary)
        {
            this.Status.Code = (int)HttpStatusCode.BadRequest;

            var errorMessages = from state in modelStateDictionary.Values
                                from error in state.Errors
                                select error.ErrorMessage;
            var errors = errorMessages.ToArray();

            this.Status.Message = string.Join("\r\n", errors);
        }
    }
}
