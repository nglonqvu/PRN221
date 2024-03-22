using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Web_PizzaShop.Hubs;
using Web_PizzaShop.Models;

namespace Web_PizzaShop.Pages.Common
{
    public class CartModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly PRN221_PRJContext _context;
        private readonly IHubContext<HubService> _hubContext;
        public List<ShoppingCartItem> shoppingCartItems { get; set; }
        public class PizzaViewModel
        {
            public Pizza pizza { get; set; }
            public Size size { get; set; }
            public CakeBasis cakeBasis { get; set; }
            public int amount { get; set; }
        }
        public List<PizzaViewModel> pizzaViewModels { get; set; }
        public CartModel(ILogger<IndexModel> logger, PRN221_PRJContext context, IHubContext<HubService> hubContext)
        {
            _logger = logger;
            _context = context;
            _hubContext = hubContext;
        }

        public async Task OnGet()
        {
            var cartCookie = Request.Cookies["Cart"];
            List<PizzaViewModel> _pizzaViewModels;
            if (cartCookie == null)
            {
                _pizzaViewModels = new List<PizzaViewModel>();
            }
            else
            {
                _pizzaViewModels = new List<PizzaViewModel>();
                var _shoppingCartItems = JsonSerializer.Deserialize<List<ShoppingCartItem>>(cartCookie);
                foreach (var item in _shoppingCartItems)
                {
                    PizzaViewModel pizzaViewModel = new PizzaViewModel()
                    {
                        pizza = await _context.Pizzas.FirstOrDefaultAsync(x => x.Id == item.PizzaId),
                        size = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == int.Parse(item.SizeId)),
                        cakeBasis = await _context.CakeBases.FirstOrDefaultAsync(x => x.Id == int.Parse(item.CakebaseId)),
                        amount = item.Amount,
                    };
                    _pizzaViewModels.Add(pizzaViewModel);
                }
                pizzaViewModels = _pizzaViewModels;
            }
        }

        public async Task<IActionResult> OnPostUpdateQuantity()
        {
            try
            {
                int sizeId = int.Parse(Request.Form["sizeId"]);
                int pizzaId = int.Parse(Request.Form["pizzaId"]);
                int cakeBaseId = int.Parse(Request.Form["cakebaseId"]);
                string action = Request.Form["action"];
                var cartCookie = Request.Cookies["Cart"];
                List<PizzaViewModel> _pizzaViewModels = new List<PizzaViewModel>();
                List<ShoppingCartItem> cartItems = new List<ShoppingCartItem>();
                cartItems = JsonSerializer.Deserialize<List<ShoppingCartItem>>(cartCookie);
                bool found = false;
                foreach (var item in cartItems)
                {
                    if (item.PizzaId == pizzaId && item.SizeId == sizeId.ToString() && item.CakebaseId == cakeBaseId.ToString() && action == "increase")
                    {
                        item.Amount++;
                        found = true;
                        break;
                    }
                    else if (item.PizzaId == pizzaId && item.SizeId == sizeId.ToString() && item.CakebaseId == cakeBaseId.ToString() && action == "decrease")
                    {
                        item.Amount--;
                        if(item.Amount == 0){
                            cartItems.Remove(item);
                        }
                        found = true;
                        break;
                    }
                }
                foreach (var item in cartItems)
                {
                    PizzaViewModel pizzaViewModel = new PizzaViewModel()
                    {
                        pizza = await _context.Pizzas.FirstOrDefaultAsync(x => x.Id == item.PizzaId),
                        size = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == int.Parse(item.SizeId)),
                        cakeBasis = await _context.CakeBases.FirstOrDefaultAsync(x => x.Id == int.Parse(item.CakebaseId)),
                        amount = item.Amount,
                    };
                    _pizzaViewModels.Add(pizzaViewModel);
                }
                pizzaViewModels = _pizzaViewModels;
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(7)
                };
                Response.Cookies.Append("Cart", JsonSerializer.Serialize(cartItems), cookieOptions);
                var jsonSerializerOptions = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    WriteIndented = true
                };
                return new JsonResult(pizzaViewModels, jsonSerializerOptions);
            }
            catch (Exception ex)
            {
                return BadRequest("Error adding item to cart");
            }
        }

        public async Task<IActionResult> OnPostDeleteitemcart()
        {
            try
            {
                int sizeId = int.Parse(Request.Form["sizeId"]);
                int pizzaId = int.Parse(Request.Form["pizzaId"]);
                int cakeBaseId = int.Parse(Request.Form["cakebaseId"]);
                var cartCookie = Request.Cookies["Cart"];
                List<PizzaViewModel> _pizzaViewModels = new List<PizzaViewModel>();
                List<ShoppingCartItem> cartItems = new List<ShoppingCartItem>();
                cartItems = JsonSerializer.Deserialize<List<ShoppingCartItem>>(cartCookie);
                bool found = false;
                foreach (var item in cartItems)
                {
                    if (item.PizzaId == pizzaId && item.SizeId == sizeId.ToString() && item.CakebaseId == cakeBaseId.ToString())
                    {
                        cartItems.Remove(item);
                        found = true;
                        break;
                    }
                }
                foreach (var item in cartItems)
                {
                    PizzaViewModel pizzaViewModel = new PizzaViewModel()
                    {
                        pizza = await _context.Pizzas.FirstOrDefaultAsync(x => x.Id == item.PizzaId),
                        size = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == int.Parse(item.SizeId)),
                        cakeBasis = await _context.CakeBases.FirstOrDefaultAsync(x => x.Id == int.Parse(item.CakebaseId)),
                        amount = item.Amount,
                    };
                    _pizzaViewModels.Add(pizzaViewModel);
                }
                pizzaViewModels = _pizzaViewModels;
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(7)
                };
                Response.Cookies.Append("Cart", JsonSerializer.Serialize(cartItems), cookieOptions);
                var jsonSerializerOptions = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    WriteIndented = true
                };
                return new JsonResult(pizzaViewModels, jsonSerializerOptions);
            }
            catch (Exception ex)
            {
                return BadRequest("Error adding item to cart");
            }
        }
    }
}
