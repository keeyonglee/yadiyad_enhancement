using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YadiYad.Pro.Web.Enums
{
    public enum NewsType
    {
        [Display(Name = "Pro News")]
        [Description("Pro News")]
        ProNews = 1,
        [Display(Name = "Pro Promotion")]
        [Description("Pro Promotion")]
        ProPromotion,
        [Display(Name = "Pro Announcement")]
        [Description("Pro Announcement")]
        ProAnnouncement,
        [Display(Name = "Shuq News")]
        [Description("Shuq News")]
        ShuqNews,
        [Display(Name = "Shuq Promotion")]
        [Description("Shuq Promotion")]
        ShuqPromotion,
        [Display(Name = "Shuq Announcement")]
        [Description("Shuq Announcement")]
        ShuqAnnouncement
    }
}
