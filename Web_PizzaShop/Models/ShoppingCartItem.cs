using System;
using System.Collections.Generic;

namespace Web_PizzaShop.Models
{
    public partial class ShoppingCartItem
    {
        public int ShoppingCartItemId { get; set; }
        public int Amount { get; set; }
        public int? PizzaId { get; set; }
        public string? SizeId { get; set; }
        public string? CakebaseId { get; set; }
        public int ShoppingCartId { get; set; }

        public virtual ShoppingCart ShoppingCart { get; set; } = null!;
    }
}
