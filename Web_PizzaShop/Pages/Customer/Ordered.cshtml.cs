using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Web_PizzaShop.Hubs;
using Web_PizzaShop.Models;

namespace Web_PizzaShop.Pages.Customer
{
    public class OrderedModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly PRN221_PRJContext _context;
        private readonly IHubContext<HubService> _hubContext;
        public OrderedModel(ILogger<IndexModel> logger, PRN221_PRJContext context, IHubContext<HubService> hubContext)
        {
            _logger = logger;
            _context = context;
            _hubContext = hubContext;
        }
        public class PizzaViewModel
        {
            public Pizza pizza { get; set; }
            public Size size { get; set; }
            public CakeBasis cakeBasis { get; set; }
            public int amount { get; set; }
        }
        public List<PizzaViewModel> pizzaViewModels { get; set; }
        public List<ShoppingCartItem> shoppingCartItems { get; set; }
        public User user1 { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var user = HttpContext.Session.GetString("account");
            if (user == null)
            {
                return RedirectToPage("../Common/LoginPage");
            }
            else
            {
                List<ShoppingCartItem> shoppingCartItems = new List<ShoppingCartItem>();
                List<PizzaViewModel> _pizzaViewModels = new List<PizzaViewModel>();
                User _user = JsonSerializer.Deserialize<User>(user);
                var CartId = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == _user.Id);
                User userr = await _context.Users.FirstOrDefaultAsync(x => x.Id == _user.Id);
                user1 = userr;
                shoppingCartItems = await _context.ShoppingCartItems.Where(x => x.ShoppingCartId == CartId.Id).ToListAsync();
                foreach (var item in shoppingCartItems)
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
            return Page();
        }

        public async Task<IActionResult> OnPostOrder()
        {
            try
            {
                decimal? total = 0;
                decimal? _total = 0;
                string name = Request.Form["name"];
                string phone = Request.Form["phone"];
                string address = Request.Form["address"];
                var user = HttpContext.Session.GetString("account");
                User _user = JsonSerializer.Deserialize<User>(user);
                ShoppingCart cart = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == _user.Id);
                List<ShoppingCartItem> shoppingCartItems = await _context.ShoppingCartItems.Where(x => x.ShoppingCartId == cart.Id).ToListAsync();
                List<PizzaViewModel> _pizzaViewModels = new List<PizzaViewModel>();
                foreach (var item in shoppingCartItems)
                {
                    PizzaViewModel pizzaViewModel = new PizzaViewModel()
                    {
                        pizza = await _context.Pizzas.FirstOrDefaultAsync(x => x.Id == item.PizzaId),
                        size = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == item.SizeId),
                        cakeBasis = await _context.CakeBases.FirstOrDefaultAsync(x => x.Id == item.CakebaseId),
                        amount = item.Amount,
                    };
                    _pizzaViewModels.Add(pizzaViewModel);
                    total += ((pizzaViewModel.pizza.Price + pizzaViewModel.size.PriceSize + pizzaViewModel.cakeBasis.PriceBase) * pizzaViewModel.amount);
                }
                Order order = new Order()
                {
                    AddressLine = address,
                    OrderPlaced = DateTime.Now,
                    OrderTotal = total,
                    PhoneNumber = phone,
                    State = "Processing",
                    UserId = _user.Id,
                    CreatedAt = DateTime.Now,
                    DeletedAt = null
                };
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                int orderId = order.OrderId;
                foreach (var item in shoppingCartItems)
                {
                    PizzaViewModel pizzaViewModel = new PizzaViewModel()
                    {
                        pizza = await _context.Pizzas.FirstOrDefaultAsync(x => x.Id == item.PizzaId),
                        size = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == item.SizeId),
                        cakeBasis = await _context.CakeBases.FirstOrDefaultAsync(x => x.Id == item.CakebaseId),
                        amount = item.Amount,
                    };

                    _total += ((pizzaViewModel.pizza.Price + pizzaViewModel.size.PriceSize + pizzaViewModel.cakeBasis.PriceBase) * pizzaViewModel.amount);

                    PizzaOrder orderDetail = new PizzaOrder()
                    {
                        OrderId = orderId,
                        PizzaId = pizzaViewModel.pizza.Id,
                        SizeId = pizzaViewModel.size.Id,
                        CakeBaseId = pizzaViewModel.cakeBasis.Id,
                        Amount = pizzaViewModel.amount,
                        Price = (double)_total,
                        Note = ""
                    };
                    await _context.PizzaOrders.AddAsync(orderDetail);
                    _total = 0;
                }
                foreach (var item in shoppingCartItems)
                {
                    _context.ShoppingCartItems.Remove(item);
                }
                _context.ShoppingCarts.Remove(cart);
                await _context.SaveChangesAsync();
                return new JsonResult("Order successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest("Error order");
            }
        }
    }
}
