namespace CityInfo.API.Services;

public class PaginationMetadata
{
    public int TotalItemCount { get; set; }
    public int TotalPageCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }

    public PaginationMetadata(int totalItemCount, int pageSize, int currentPage)
    {
        TotalItemCount = totalItemCount;
        TotalPageCount = (int)Math.Ceiling(TotalItemCount / (double)pageSize);
        PageSize = pageSize;
        CurrentPage = currentPage;
    }
}