using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Web_PizzaShop.Hubs;
using Web_PizzaShop.Models;

namespace Web_PizzaShop.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly PRN221_PRJContext _context;
        private readonly IHubContext<HubService> _hubContext;
        public List<Pizza> pizza_hots {get; set;}
        public int totalpizza {get; set;}
        public IndexModel(ILogger<IndexModel> logger, PRN221_PRJContext context, IHubContext<HubService> hubContext)
        {
            _logger = logger;
            _context = context;
            _hubContext = hubContext;
        }

        public async Task OnGet()
        {
            try
            {
                pizza_hots = await _context.Pizzas.Where(p => p.IsPizzaOfTheWeek == true).ToListAsync();
                var pizzas = await _context.Pizzas.ToListAsync();
                totalpizza = pizzas.Count();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }
    }
}
