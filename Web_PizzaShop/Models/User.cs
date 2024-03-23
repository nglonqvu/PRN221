using System;
using System.Collections.Generic;

namespace Web_PizzaShop.Models
{
    public partial class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
            Reviews = new HashSet<Review>();
            ShoppingCarts = new HashSet<ShoppingCart>();
            Roles = new HashSet<Role>();
        }

        public int Id { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserName { get; set; }
        public string? Address { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
