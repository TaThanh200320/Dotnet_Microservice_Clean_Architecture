
using System.Collections.Generic;

namespace SharedKernel.Models;

public class PaginationResponse<T>
{
    public IEnumerable<T>? Data { get; private set; }

    public Paging<T>? Paging { get; private set; }

    public PaginationResponse(IEnumerable<T> data, int totalItemCount, int currentPage, int pageSize)
    {
        Data = data;
        Paging = new Paging<T>(totalItemCount, currentPage, pageSize);
    }

    public PaginationResponse(IEnumerable<T> data, int totalItemCount, int pageSize, string? previousCursor = null, string? nextCursor = null)
    {
        Data = data;
        Paging = new Paging<T>(totalItemCount, pageSize, previousCursor, nextCursor);
    }
}