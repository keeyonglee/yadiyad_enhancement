using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YadiYad.Pro.Web.DTO.Base
{
    public class DataTableAjaxRequestDTO<T>
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public List<DataTableColumn> Columns { get; set; }
        public DataTableSearch Search { get; set; }
        public List<DataTableOrder> Order { get; set; }
        public T CustomFilter { get; set; }
    }

    public class DataTableColumn
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public DataTableSearch Search { get; set; }
    }

    public class DataTableSearch
    {
        public string Value { get; set; }
        public string Regex { get; set; }
    }

    public class DataTableOrder
    {
        public int Column { get; set; }
        public string dir { get; set; }
        public int Dir
        {
            get
            {
                return (int)Enum.Parse(typeof(OrderDirection), dir, true);
            }
        }
    }
}
