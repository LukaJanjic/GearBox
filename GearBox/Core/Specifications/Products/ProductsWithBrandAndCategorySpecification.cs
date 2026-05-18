using Core.Domain.Entities;
using Core.RequestHelpers;

namespace Core.Specifications.Products;

public class ProductsWithBrandAndCategorySpecification : BaseSpecification<Product>
{
    public ProductsWithBrandAndCategorySpecification()
    {
        AddInclude(p => p.Brand!);
        AddInclude(p => p.Category!);
    }

    public ProductsWithBrandAndCategorySpecification(int id) : this()
    {
        AddCriteria(p => p.Id == id);
    }

    public ProductsWithBrandAndCategorySpecification(ProductQueryParams queryParams) : this()
    {
        var brands     = queryParams.Brands is { Count: > 0 } ? queryParams.Brands : null;
        var categories = queryParams.Categories is { Count: > 0 } ? queryParams.Categories : null;
        var search     = queryParams.Search;
        var minPrice   = queryParams.MinPrice;
        var maxPrice   = queryParams.MaxPrice;

        AddCriteria(p =>
            (brands == null     || brands.Contains(p.Brand!.Name!)) &&
            (categories == null || categories.Contains(p.Category!.Name!)) &&
            (search == null     || p.Name!.Contains(search)) &&
            (minPrice == null   || p.Price >= minPrice) &&
            (maxPrice == null   || p.Price <= maxPrice)
        );
        switch (queryParams.Sort)
        {
            case "priceAsc":  AddOrderBy(p => p.Price); break;
            case "priceDesc": AddOrderByDescending(p => p.Price); break;
            case "nameAsc":   AddOrderBy(p => p.Name!); break;
            case "nameDesc":  AddOrderByDescending(p => p.Name!); break;
            default:          AddOrderBy(p => p.Name!); break;
        }

        ApplyPaging(queryParams.PageIndex, queryParams.PageSize);
    }
}
