using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace YadiYad.Pro.Web.Models.Customer
{
    public partial class SegmentContactUsModel : BaseNopModel
    {
        public SegmentContactUsModel()
        {
        }

        public string Segment { get; set; }
        public string Name { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

    }
}