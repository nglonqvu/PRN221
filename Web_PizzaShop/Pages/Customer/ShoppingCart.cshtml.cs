using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Web_PizzaShop.Models;

namespace Web_PizzaShop.Pages.Customer
{
    public class ShoppingCartModel : PageModel
    {
        private readonly PRN221_PRJContext _context;
        public String Username;
        public ShoppingCartModel(PRN221_PRJContext context)
        {
            _context = context;
        }
        public List<ShoppingCartItem> Items { get; set; }
        public async void OnGet()
        {
            Username = HttpContext.Session.GetString("username");
            if (Username == null) {
                Username = "";
            };
            Items = _context.ShoppingCartItems.Include(x => x.ShoppingCart).Where(x => x.ShoppingCartId == x.ShoppingCart.Id)
             .ToList();

           
        }
    
    }

    
}
