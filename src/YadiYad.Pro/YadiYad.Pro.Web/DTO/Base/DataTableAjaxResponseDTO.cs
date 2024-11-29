using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YadiYad.Pro.Web.DTO.Base
{
    public class DataTableAjaxResponseDTO<T, U>
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public List<T> Rata { get; set; }
        public U MoreData { get; set; }
    }
}
