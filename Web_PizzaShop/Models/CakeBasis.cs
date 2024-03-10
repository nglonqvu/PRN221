using System;
using System.Collections.Generic;

namespace Web_PizzaShop.Models
{
    public partial class CakeBasis
    {
        public CakeBasis()
        {
            PizzaOptions = new HashSet<PizzaOption>();
        }

        public int Id { get; set; }
        public decimal? PriceBase { get; set; }
        public string? CakeBase { get; set; }

        public virtual ICollection<PizzaOption> PizzaOptions { get; set; }
    }
}
