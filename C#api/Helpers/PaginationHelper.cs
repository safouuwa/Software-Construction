using System;
using System.Collections.Generic;
using System.Linq;

public static class PaginationHelper
{
    public static PaginatedResponse<T> Paginate<T>(IEnumerable<T> source, int page, int pageSize)
    {
        var totalItems = source.Count();
        var paginatedItems = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new PaginatedResponse<T>
        {
            TotalCount = totalItems,
            Page = page,
            PageSize = pageSize,
            Items = paginatedItems
        };
    }
}

public class PaginatedResponse<T>
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<T> Items { get; set; }
}