using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Common
{
    public class JobServiceCategory : BaseEntity
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public bool Published { get; set; }

    }
}
