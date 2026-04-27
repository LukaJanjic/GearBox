using System;
using Core.Domain.Entities;

namespace Core.Specifications.Brands;

public class BrandsSpecification : BaseSpecification<Brand>
{
    public BrandsSpecification()
    {
    }

    public BrandsSpecification(int id) : this()
    {
        AddCriteria(b => b.Id == id);
    }
}
