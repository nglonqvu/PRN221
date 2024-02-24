using System;
using System.Collections.Generic;

namespace Web_PizzaShop.Models
{
    public partial class ContractDetail
    {
        public int ContractId { get; set; }
        public int IngredientId { get; set; }

        public virtual SupplierContract Contract { get; set; } = null!;
        public virtual Ingredient Ingredient { get; set; } = null!;
    }
}
