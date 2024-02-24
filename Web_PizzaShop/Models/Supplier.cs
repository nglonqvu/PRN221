using System;
using System.Collections.Generic;

namespace Web_PizzaShop.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            SupplierContracts = new HashSet<SupplierContract>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }

        public virtual ICollection<SupplierContract> SupplierContracts { get; set; }
    }
}
