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
            UserTokens = new HashSet<UserToken>();
            Roles = new HashSet<Role>();
        }

        public string Id { get; set; } = null!;
        public int AccessFailedCount { get; set; }
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public string? PasswordHash { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserName { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }
        public virtual ICollection<UserToken> UserTokens { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
