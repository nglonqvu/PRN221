﻿using System;
using System.Collections.Generic;

namespace Web_PizzaShop.Models
{
    public partial class ShoppingCart
    {
        public ShoppingCart()
        {
            ShoppingCartItems = new HashSet<ShoppingCartItem>();
        }

        public int Id { get; set; }
        public string UserId { get; set; } = null!;

        public virtual User User { get; set; } = null!;
        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }
    }
}
