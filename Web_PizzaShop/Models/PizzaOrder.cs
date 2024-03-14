using System;
using System.Collections.Generic;

namespace Web_PizzaShop.Models
{
    public partial class PizzaOrder
    {
        public int PiOrderId { get; set; }
        public int OrderId { get; set; }
        public int PizzaId { get; set; }
        public int? SizeId { get; set; }
        public int? CakeBaseId { get; set; }
        public string? Note { get; set; }

        public virtual CakeBasis? CakeBase { get; set; }
        public virtual Order Order { get; set; } = null!;
        public virtual Pizza Pizza { get; set; } = null!;
        public virtual Size? Size { get; set; }
    }
}
