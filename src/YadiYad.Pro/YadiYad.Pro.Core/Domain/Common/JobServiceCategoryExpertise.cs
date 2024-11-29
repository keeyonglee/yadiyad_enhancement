using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Common
{
    public class JobServiceCategoryExpertise : BaseEntity
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        public List<Expertise> Expertises { get; set; }
    }
}
