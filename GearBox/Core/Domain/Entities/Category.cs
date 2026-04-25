namespace Core.Domain.Entities;

    public  class Category : BaseEntity
     {
        public string? Description { get; set; }
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
     }

