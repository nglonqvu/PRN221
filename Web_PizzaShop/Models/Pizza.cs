using System;
using System.Collections.Generic;

namespace Web_PizzaShop.Models
{
    public partial class Pizza
    {
        public Pizza()
        {
            PizzaIngredients = new HashSet<PizzaIngredient>();
            PizzaOptions = new HashSet<PizzaOption>();
            PizzaOrders = new HashSet<PizzaOrder>();
            Reviews = new HashSet<Review>();
        }

        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public bool IsPizzaOfTheWeek { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int CategoriesId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Category Categories { get; set; } = null!;
        public virtual ICollection<PizzaIngredient> PizzaIngredients { get; set; }
        public virtual ICollection<PizzaOption> PizzaOptions { get; set; }
        public virtual ICollection<PizzaOrder> PizzaOrders { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
