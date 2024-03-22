using System;
using System.Collections.Generic;

namespace Web_PizzaShop.Models
{
    public partial class Order
    {
        public Order()
        {
            PizzaOrders = new HashSet<PizzaOrder>();
        }

        public int OrderId { get; set; }
        public string AddressLine { get; set; } = null!;
        public DateTime OrderPlaced { get; set; }
        public decimal? OrderTotal { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string? State { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<PizzaOrder> PizzaOrders { get; set; }
    }
}
