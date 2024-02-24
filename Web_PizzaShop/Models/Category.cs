using System;
using System.Collections.Generic;

namespace Web_PizzaShop.Models
{
    public partial class Category
    {
        public Category()
        {
            Pizzas = new HashSet<Pizza>();
        }

        public int Id { get; set; }
        public string? Description { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Pizza> Pizzas { get; set; }
    }
}
