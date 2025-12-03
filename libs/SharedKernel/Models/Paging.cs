using System;
using System.Text.Json.Serialization;

namespace SharedKernel.Models;

public class Paging<T>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? CurrentPage { get; set; }

    public int PageSize { get; set; }

    public int TotalPage { get; set; }

    public bool? HasNextPage { get; set; }

    public bool? HasPreviousPage { get; set; }

    public string? Before { get; set; }

    public string? After { get; set; }

    public Paging(int totalItemCount, int currentPage = 1, int pageSize = 10)
    {
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalPage = (int)Math.Ceiling((double)totalItemCount / (double)pageSize);
        HasNextPage = CurrentPage < TotalPage;
        HasPreviousPage = currentPage > 1;
    }

    public Paging(int totalItemCount, int pageSize = 10, string? previousCursor = null, string? nextCursor = null)
    {
        PageSize = pageSize;
        TotalPage = (int)Math.Ceiling((double)totalItemCount / (double)pageSize);
        After = nextCursor;
        HasNextPage = nextCursor != null;
        Before = previousCursor;
        HasPreviousPage = previousCursor != null;
    }
}