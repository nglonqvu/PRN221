using System;
using System.Collections.Generic;

namespace Web_PizzaShop.Models
{
    public partial class PizzaOption
    {
        public int OptionId { get; set; }
        public int PizzaId { get; set; }
        public int SizeId { get; set; }
        public int CakeBaseId { get; set; }

        public virtual CakeBasis CakeBase { get; set; } = null!;
        public virtual Pizza Pizza { get; set; } = null!;
        public virtual Size Size { get; set; } = null!;
    }
}
