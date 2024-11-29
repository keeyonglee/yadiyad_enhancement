using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YadiYad.Pro.Services.DTO.Common
{
    public class PagedListDTO<T>
    {
        /// <summary>
        /// Gets or sets data records
        /// </summary>
        public IEnumerable<T> Data { get; set; }

        /// <summary>
        /// Page index
        /// </summary>
        public int PageIndex { get; }

        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Total count
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// Has previous page
        /// </summary>
        public bool HasPreviousPage { get; }

        /// <summary>
        /// Has next age
        /// </summary>
        public bool HasNextPage { get; }

        public object AdditionalData { set; get; }

        public PagedListDTO(IPagedList<T> data)
        {
            Data = data;
            PageIndex = data.PageIndex;
            PageSize = data.PageSize;
            TotalCount = data.TotalCount;
            HasPreviousPage = data.HasPreviousPage;
            HasNextPage = data.HasNextPage;
        }

        public PagedListDTO(List<T> data, int pageIndex, int pageSize, int totalCount)
        {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
            HasPreviousPage = pageIndex > 0;
            HasNextPage = totalCount > (pageIndex * pageSize + pageSize);
        }
    }
}
