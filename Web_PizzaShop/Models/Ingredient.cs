using System;
using System.Collections.Generic;

namespace Web_PizzaShop.Models
{
    public partial class Ingredient
    {
        public Ingredient()
        {
            PizzaIngredients = new HashSet<PizzaIngredient>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<PizzaIngredient> PizzaIngredients { get; set; }
    }
}
