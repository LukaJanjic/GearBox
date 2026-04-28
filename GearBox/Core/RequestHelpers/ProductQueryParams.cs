namespace Core.RequestHelpers;

public class ProductQueryParams
{
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public string? Search { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Sort { get; set; }  // priceAsc | priceDesc | nameAsc | nameDesc
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
