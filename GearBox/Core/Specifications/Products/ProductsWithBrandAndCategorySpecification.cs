using Core.Domain.Entities;

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
}
