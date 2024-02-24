using System;
using System.Collections.Generic;

namespace Web_PizzaShop.Models
{
    public partial class SupplierContract
    {
        public int Id { get; set; }
        public int? SupplierId { get; set; }
        public int? IngredientId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual Supplier? Supplier { get; set; }
    }
}
