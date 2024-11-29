

namespace YadiYad.Pro.Web.DTO.Base
{
    public class ListFilterDTO<T>
    {
        public string Filter { get; set; }
        public int Offset { get; set; }
        public int RecordSize { get; set; } = int.MaxValue;
        public ListSortingDTO Sorting { get; set; }
        public T AdvancedFilter { get; set; }
        public int SortBy { get; set; }
    }
}
