using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Common
{
    public class Expertise : BaseEntity
    {
        public string Name { get; set; }
        public int JobServiceCategoryId { get; set; }
        public bool IsOthers { get; set; }
        public bool Published { get; set; }
    }
}
