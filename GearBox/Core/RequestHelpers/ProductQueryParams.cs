namespace Core.RequestHelpers;

public class ProductQueryParams
{
    public List<string>? Brands { get; set; }
    public List<string>? Categories { get; set; }
    public string? Search { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Sort { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 9;
}
