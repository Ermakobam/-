using System.Collections.Generic;

namespace Shop.Core.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
