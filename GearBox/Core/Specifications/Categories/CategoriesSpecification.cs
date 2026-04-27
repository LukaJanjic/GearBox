using System;
using Core.Domain.Entities;

namespace Core.Specifications.Categories;

public class CategoriesSpecification : BaseSpecification<Category>
{
    public CategoriesSpecification() { }
    public CategoriesSpecification(int id): this()
    {
        AddCriteria(c => c.Id == id);
    }
}
