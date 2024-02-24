﻿using System;
using System.Collections.Generic;

namespace Web_PizzaShop.Models
{
    public partial class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int Amount { get; set; }
        public int OrderId { get; set; }
        public decimal Price { get; set; }

        public virtual Order Order { get; set; } = null!;
    }
}
