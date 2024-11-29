using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Pro.Models.YadiyadNews
{
    public class YadiyadNewsModel : BaseNopEntityModel
    {
        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public int LanguageId { get; set; } = 1;

        /// <summary>
        /// Gets or sets the news title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the short text
        /// </summary>
        public string Short { get; set; }

        /// <summary>
        /// Gets or sets the full text
        /// </summary>
        public string Full { get; set; }

        /// <summary>
        /// Gets or sets the news item start date and time
        /// </summary>
        /// 
        [NopResourceDisplayName("Start Date")]
        public DateTime? StartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the news item end date and time
        /// </summary>
        /// 
        [NopResourceDisplayName("End Date")]
        public DateTime? EndDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
        [NopResourceDisplayName("Type")]
        public int NewsTypeId { get; set; }
        public string NewsTypeString { get; set; }
    }
}
