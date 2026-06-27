namespace TaskManagement.Common;

public class Pagination
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public Pagination Pagination { get; set; } = new();
}
