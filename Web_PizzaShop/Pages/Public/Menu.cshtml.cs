
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Web_PizzaShop.Hubs;
using Web_PizzaShop.Models;

namespace Web_PizzaShop.Pages.Public
{
    public class MenuModel : PageModel
    {
        public class PizzaViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public string Description { get; set; }
            public string? ImageUrl { get; set; }
            public decimal Total { get; set; }
            public List<Size> Sizes { get; set; }
            public List<CakeBasis> CakeBases { get; set; }
        }

        public class CakeBaseData
        {
            public List<CakeBasis> CakeBases { get; set; }
            public decimal Total { get; set; }
        }

        public class CartSession
        {
            public int Amount { get; set; }
            public int? PizzaId { get; set; }
            public string? SizeId { get; set; }
            public string? CakebaseId { get; set; }
        }
        private readonly ILogger<IndexModel> _logger;
        private readonly PRN221_PRJContext _context;
        private readonly IHubContext<HubService> _hubContext;
        public List<Pizza> pizzas { get; set; }
        public int totalpizza { get; set; }
        public List<PizzaViewModel> _pizzaViewModels { get; set; }
        public MenuModel(ILogger<IndexModel> logger, PRN221_PRJContext context, IHubContext<HubService> hubContext)
        {
            _logger = logger;
            _context = context;
            _hubContext = hubContext;
        }

        public async Task OnGet()
        {
            var _pizzas = await _context.Pizzas.Include(x => x.PizzaOptions).OrderByDescending(x => x.CreatedAt).ToListAsync();
            var _size = await _context.Sizes.OrderBy(x => x.Id).ToListAsync();
            Dictionary<int, Size> sizeMap = new Dictionary<int, Size>();
            sizeMap = _size.ToDictionary(c => c.Id);
            var _cakebase = await _context.CakeBases.ToListAsync();
            Dictionary<int, CakeBasis> cakebaseMap = new Dictionary<int, CakeBasis>();
            cakebaseMap = _cakebase.ToDictionary(c => c.Id);
            Dictionary<int, List<CakeBasis>> sizeCakeBaseMap = new Dictionary<int, List<CakeBasis>>();
            List<PizzaViewModel> pizzaViewModels = new List<PizzaViewModel>();
            foreach (var pizza in _pizzas)
            {
                decimal total = 0;
                PizzaViewModel pizzaViewModel = new PizzaViewModel();
                pizzaViewModel.Id = pizza.Id;
                pizzaViewModel.Name = pizza.Name;
                pizzaViewModel.Price = pizza.Price;
                pizzaViewModel.Description = pizza.Description;
                pizzaViewModel.ImageUrl = pizza.ImageUrl;
                pizzaViewModel.Sizes = new List<Size>();
                pizzaViewModel.CakeBases = new List<CakeBasis>();
                total += pizza.Price;
                var SizeIds = pizza.PizzaOptions.OrderBy(x => x.SizeId).Select(x => x.SizeId).Distinct().ToList();
                foreach (var sizeId in SizeIds)
                {
                    if (sizeMap.TryGetValue(sizeId, out var size))
                    {
                        pizzaViewModel.Sizes.Add(size);
                    }
                }
                var firstSizeId = pizza.PizzaOptions.OrderBy(x => x.SizeId).Select(x => x.SizeId).FirstOrDefault();
                if (sizeMap.TryGetValue(firstSizeId, out var sizee))
                {
                    total += (decimal)sizee.PriceSize;
                }
                foreach (var sizeId in SizeIds)
                {
                    if (sizeMap.TryGetValue(sizeId, out var size))
                    {
                        var _CakeBases = pizza.PizzaOptions.Where(X => X.SizeId == sizeId).OrderBy(x => x.SizeId).Select(x => x.CakeBaseId).Distinct().ToList();
                        foreach (var cakeBaseId in _CakeBases)
                        {
                            if (cakebaseMap.TryGetValue((int)cakeBaseId, out CakeBasis cakeBasis))
                            {
                                pizzaViewModel.CakeBases.Add(cakeBasis);
                            }
                        }
                    }
                    var firstcaksebaseId = pizza.PizzaOptions.Where(X => X.SizeId == sizeId).OrderBy(x => x.SizeId).Select(x => x.CakeBaseId).FirstOrDefault();
                    if (cakebaseMap.TryGetValue((int)firstcaksebaseId, out CakeBasis _cakeBasis))
                    {
                        total += (decimal)_cakeBasis.PriceBase;
                    }
                    break;
                }
                pizzaViewModel.Total = total;
                pizzaViewModels.Add(pizzaViewModel);
            }
            _pizzaViewModels = pizzaViewModels;
        }

        public async Task<IActionResult> OnPostReloadPizzaBySize()
        {
            string sizeId = Request.Form["size"];
            string pizzaId = Request.Form["pizzaId"];
            decimal total = 0;
            List<int> cakebaseId = new List<int>();
            Pizza pizza = _context.Pizzas.Include(x => x.PizzaOptions).FirstOrDefault(x => x.Id == int.Parse(pizzaId));
            total += pizza.Price;
            foreach (var cb in pizza.PizzaOptions)
            {
                if (cb.SizeId == int.Parse(sizeId))
                {
                    cakebaseId.Add(cb.CakeBaseId);
                }
            }
            cakebaseId = cakebaseId.Distinct().ToList();
            Size size = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == int.Parse(sizeId));
            total += (decimal)size.PriceSize;
            List<CakeBasis> CakeBases = await _context.CakeBases.Where(cb => cakebaseId.Contains(cb.Id)).ToListAsync();
            total += (decimal)CakeBases[0].PriceBase;
            var cakeBaseData = new CakeBaseData
            {
                CakeBases = CakeBases,
                Total = total
            };
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };
            return new JsonResult(cakeBaseData, jsonSerializerOptions);
        }

        public async Task<IActionResult> OnPostReloadPizzaByCakeBase()
        {
            int sizeId = int.Parse(Request.Form["size"]);
            int pizzaId = int.Parse(Request.Form["pizzaId"]);
            int cakeBaseId = int.Parse(Request.Form["cakeBaseId"]);
            decimal total = 0;
            List<int> cakebaseId = new List<int>();
            Pizza pizza = await _context.Pizzas.Include(x => x.PizzaOptions).FirstOrDefaultAsync(x => x.Id == pizzaId);
            total += pizza.Price;
            Size size = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == sizeId);
            total += (decimal)size.PriceSize;
            List<CakeBasis> CakeBases = await _context.CakeBases.Where(cb => cb.Id == cakeBaseId).ToListAsync();
            total += (decimal)CakeBases[0].PriceBase;
            var cakeBaseData = new CakeBaseData
            {
                CakeBases = CakeBases,
                Total = total
            };
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };
            return new JsonResult(cakeBaseData, jsonSerializerOptions);
        }

        public async Task<IActionResult> OnPostAddToCart()
        {
            try
            {
                int sizeId = int.Parse(Request.Form["size"]);
                int pizzaId = int.Parse(Request.Form["pizzaId"]);
                int cakeBaseId = int.Parse(Request.Form["cakebase"]);
                var cartCookie = Request.Cookies["Cart"];
                List<ShoppingCartItem> cartItems;

                if (cartCookie == null)
                {
                    cartItems = new List<ShoppingCartItem>();
                }
                else
                {
                    cartItems = JsonSerializer.Deserialize<List<ShoppingCartItem>>(cartCookie);
                }
                Console.WriteLine(cartItems.Count());
                bool found = false;
                foreach (var item in cartItems)
                {
                    if (item.PizzaId == pizzaId && item.SizeId == sizeId.ToString() && item.CakebaseId == cakeBaseId.ToString())
                    {
                        item.Amount++;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    var newItem = new ShoppingCartItem
                    {
                        ShoppingCartId = -1,
                        ShoppingCartItemId = -1,
                        PizzaId = pizzaId,
                        SizeId = sizeId.ToString(),
                        CakebaseId = cakeBaseId.ToString(),
                        Amount = 1
                    };
                    cartItems.Add(newItem);
                }

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(7)
                };
                Response.Cookies.Append("Cart", JsonSerializer.Serialize(cartItems), cookieOptions);
                return new JsonResult("Item added to cart successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest("Error adding item to cart");
            }
        }

    }
}
