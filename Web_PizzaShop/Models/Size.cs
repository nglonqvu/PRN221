using System;
using System.Collections.Generic;

namespace Web_PizzaShop.Models
{
    public partial class Size
    {
        public Size()
        {
            PizzaOptions = new HashSet<PizzaOption>();
            PizzaOrders = new HashSet<PizzaOrder>();
        }

        public int Id { get; set; }
        public decimal? PriceSize { get; set; }
        public string? Size1 { get; set; }

        public virtual ICollection<PizzaOption> PizzaOptions { get; set; }
        public virtual ICollection<PizzaOrder> PizzaOrders { get; set; }
    }
}
