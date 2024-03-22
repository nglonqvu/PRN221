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
            var user = HttpContext.Session.GetString("account");
            List<ShoppingCartItem> _shoppingCartItems = new List<ShoppingCartItem>();
            List<PizzaViewModel> _pizzaViewModels = new List<PizzaViewModel>();
            if (user == null)
            {
                var cartCookie = Request.Cookies["Cart"];

                if (cartCookie == null)
                {
                    _pizzaViewModels = new List<PizzaViewModel>();
                }
                else
                {
                    _shoppingCartItems = JsonSerializer.Deserialize<List<ShoppingCartItem>>(cartCookie);
                }
            }
            else
            {
                User _user = JsonSerializer.Deserialize<User>(user);
                ShoppingCart CartId = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == _user.Id);
                if(CartId == null){
                    CartId = new ShoppingCart(){
                        UserId = _user.Id,
                    };
                    await _context.ShoppingCarts.AddAsync(CartId);
                    await _context.SaveChangesAsync();
                }
                var cartCookie = Request.Cookies["Cart"];
                if (cartCookie == null)
                {
                    _shoppingCartItems = await _context.ShoppingCartItems.Where(x => x.ShoppingCartId == CartId.Id).ToListAsync();
                }
                else
                {
                    _shoppingCartItems = await _context.ShoppingCartItems.Where(x => x.ShoppingCartId == CartId.Id).ToListAsync();
                    List<ShoppingCartItem> shoppingCartItemsToRemove = new List<ShoppingCartItem>();
                    List<ShoppingCartItem> shoppingCartItems = JsonSerializer.Deserialize<List<ShoppingCartItem>>(cartCookie);
                    foreach (ShoppingCartItem _item in _shoppingCartItems)
                    {
                        foreach (ShoppingCartItem item in shoppingCartItems)
                        {
                            if (_item.PizzaId == item.PizzaId && _item.SizeId == item.SizeId && _item.CakebaseId == item.CakebaseId)
                            {
                                _item.Amount++;
                                shoppingCartItemsToRemove.Add(item);
                            }
                        }
                    }

                    foreach (var itemToRemove in shoppingCartItemsToRemove)
                    {
                        shoppingCartItems.Remove(itemToRemove);
                        var cookieOptions = new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(7)
                        }; Response.Cookies.Append("Cart", JsonSerializer.Serialize(shoppingCartItems), cookieOptions);
                    }
                    _context.SaveChanges();
                }
            }
            foreach (var item in _shoppingCartItems)
            {
                PizzaViewModel pizzaViewModel = new PizzaViewModel()
                {
                    pizza = await _context.Pizzas.FirstOrDefaultAsync(x => x.Id == item.PizzaId),
                    size = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == item.SizeId),
                    cakeBasis = await _context.CakeBases.FirstOrDefaultAsync(x => x.Id == item.CakebaseId),
                    amount = item.Amount,
                };
                _pizzaViewModels.Add(pizzaViewModel);
            }
            pizzaViewModels = _pizzaViewModels;
        }

        public async Task<IActionResult> OnPostUpdateQuantity()
        {
            try
            {
                int sizeId = int.Parse(Request.Form["sizeId"]);
                int pizzaId = int.Parse(Request.Form["pizzaId"]);
                int cakeBaseId = int.Parse(Request.Form["cakebaseId"]);
                string action = Request.Form["action"];
                var user = HttpContext.Session.GetString("account");
                List<PizzaViewModel> _pizzaViewModels = new List<PizzaViewModel>();
                List<ShoppingCartItem> cartItems = new List<ShoppingCartItem>();
                if (user == null)
                {
                    var cartCookie = Request.Cookies["Cart"];
                    cartItems = JsonSerializer.Deserialize<List<ShoppingCartItem>>(cartCookie);
                    bool found = false;
                    foreach (var item in cartItems)
                    {
                        if (item.PizzaId == pizzaId && item.SizeId == sizeId && item.CakebaseId == cakeBaseId && action == "increase")
                        {
                            item.Amount++;
                            found = true;
                            break;
                        }
                        else if (item.PizzaId == pizzaId && item.SizeId == sizeId && item.CakebaseId == cakeBaseId && action == "decrease")
                        {
                            item.Amount--;
                            if (item.Amount == 0)
                            {
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
                            size = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == item.SizeId),
                            cakeBasis = await _context.CakeBases.FirstOrDefaultAsync(x => x.Id == item.CakebaseId),
                            amount = item.Amount,
                        };
                        _pizzaViewModels.Add(pizzaViewModel);
                    }
                    pizzaViewModels = _pizzaViewModels;
                }
                else
                {
                    User _user = JsonSerializer.Deserialize<User>(user);
                    var CartId = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == _user.Id);
                    cartItems = await _context.ShoppingCartItems.Where(x => x.ShoppingCartId == CartId.Id).ToListAsync();
                    bool found = false;
                    foreach (var item in cartItems)
                    {
                        if (item.PizzaId == pizzaId && item.SizeId == sizeId && item.CakebaseId == cakeBaseId && action == "increase")
                        {
                            item.Amount++;
                            found = true;
                            break;
                        }
                        else if (item.PizzaId == pizzaId && item.SizeId == sizeId && item.CakebaseId == cakeBaseId && action == "decrease")
                        {
                            item.Amount--;
                            if (item.Amount == 0)
                            {
                                cartItems.Remove(item);
                                _context.ShoppingCartItems.Remove(item);
                                await _context.SaveChangesAsync();
                            }
                            found = true;
                            break;
                        }
                    }
                    _context.SaveChanges();
                    foreach (var item in cartItems)
                    {
                        PizzaViewModel pizzaViewModel = new PizzaViewModel()
                        {
                            pizza = await _context.Pizzas.FirstOrDefaultAsync(x => x.Id == item.PizzaId),
                            size = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == item.SizeId),
                            cakeBasis = await _context.CakeBases.FirstOrDefaultAsync(x => x.Id == item.CakebaseId),
                            amount = item.Amount,
                        };
                        _pizzaViewModels.Add(pizzaViewModel);
                    }
                    pizzaViewModels = _pizzaViewModels;
                }
                if (user == null)
                {
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddMinutes(7)
                    };
                    Response.Cookies.Append("Cart", JsonSerializer.Serialize(cartItems), cookieOptions);
                }
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
                var user = HttpContext.Session.GetString("account");
                List<PizzaViewModel> _pizzaViewModels = new List<PizzaViewModel>();
                List<ShoppingCartItem> cartItems = new List<ShoppingCartItem>();
                if (user == null)
                {
                    var cartCookie = Request.Cookies["Cart"];
                    cartItems = JsonSerializer.Deserialize<List<ShoppingCartItem>>(cartCookie);
                    bool found = false;
                    foreach (var item in cartItems)
                    {
                        if (item.PizzaId == pizzaId && item.SizeId == sizeId && item.CakebaseId == cakeBaseId)
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
                            size = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == item.SizeId),
                            cakeBasis = await _context.CakeBases.FirstOrDefaultAsync(x => x.Id == item.CakebaseId),
                            amount = item.Amount,
                        };
                        _pizzaViewModels.Add(pizzaViewModel);
                    }
                    pizzaViewModels = _pizzaViewModels;
                }
                else
                {
                    User _user = JsonSerializer.Deserialize<User>(user);
                    var CartId = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == _user.Id);
                    cartItems = await _context.ShoppingCartItems.Where(x => x.ShoppingCartId == CartId.Id).ToListAsync();
                    bool found = false;
                    foreach (var item in cartItems)
                    {
                        if (item.PizzaId == pizzaId && item.SizeId == sizeId && item.CakebaseId == cakeBaseId)
                        {
                            cartItems.Remove(item);
                            _context.ShoppingCartItems.Remove(item);                    
                            _context.SaveChanges();
                            found = true;
                            break;
                        }
                    }

                    foreach (var item in cartItems)
                    {
                        PizzaViewModel pizzaViewModel = new PizzaViewModel()
                        {
                            pizza = await _context.Pizzas.FirstOrDefaultAsync(x => x.Id == item.PizzaId),
                            size = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == item.SizeId),
                            cakeBasis = await _context.CakeBases.FirstOrDefaultAsync(x => x.Id == item.CakebaseId),
                            amount = item.Amount,
                        };
                        _pizzaViewModels.Add(pizzaViewModel);
                    }
                    pizzaViewModels = _pizzaViewModels;
                }
                if (user == null)
                {
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddMinutes(7)
                    };
                    Response.Cookies.Append("Cart", JsonSerializer.Serialize(cartItems), cookieOptions);
                }
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
