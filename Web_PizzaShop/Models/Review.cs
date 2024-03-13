using System;
using System.Collections.Generic;

namespace Web_PizzaShop.Models
{
    public partial class Review
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = null!;
        public int Grade { get; set; }
        public int PizzaId { get; set; }
        public string Title { get; set; } = null!;
        public int? UserId { get; set; }

        public virtual Pizza Pizza { get; set; } = null!;
        public virtual User? User { get; set; }
    }
}
