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
        AddCriteria(p =>
            (string.IsNullOrEmpty(queryParams.Brand) || p.Brand!.Name == queryParams.Brand) &&
            (string.IsNullOrEmpty(queryParams.Category) || p.Category!.Name == queryParams.Category) &&
            (string.IsNullOrEmpty(queryParams.Search) || p.Name!.Contains(queryParams.Search)) &&
            (queryParams.MinPrice == null || p.Price >= queryParams.MinPrice) &&
            (queryParams.MaxPrice == null || p.Price <= queryParams.MaxPrice)
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
