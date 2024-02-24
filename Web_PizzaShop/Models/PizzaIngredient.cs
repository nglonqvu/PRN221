using System;
using System.Collections.Generic;

namespace Web_PizzaShop.Models
{
    public partial class PizzaIngredient
    {
        public int Id { get; set; }
        public int IngredientId { get; set; }
        public int PizzaId { get; set; }

        public virtual Ingredient Ingredient { get; set; } = null!;
        public virtual Pizza Pizza { get; set; } = null!;
    }
}
