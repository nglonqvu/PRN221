using System.Text.Json;
using System.Text.Json.Serialization;
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
            public List<Size> Sizes { get; set; }
            public Dictionary<int, List<CakeBasis>> sizeCakeBase { get; set; }

        }
        private readonly ILogger<IndexModel> _logger;
        private readonly PRN221_PRJContext _context;
        private readonly IHubContext<HubService> _hubContext;
        public List<Pizza> pizzas { get; set; }
        public int totalpizza { get; set; }
        public List<PizzaViewModel> _pizzaViewModels {get; set;}
        public MenuModel(ILogger<IndexModel> logger, PRN221_PRJContext context, IHubContext<HubService> hubContext)
        {
            _logger = logger;
            _context = context;
            _hubContext = hubContext;
        }



        public async Task OnGet()
        {
            var _pizzas = await _context.Pizzas.Include(x => x.PizzaOptions).OrderByDescending(x => x.CreatedAt).ToListAsync();
            var _size = await _context.Sizes.ToListAsync();
            Dictionary<int, Size> sizeMap = new Dictionary<int, Size>();
            sizeMap = _size.ToDictionary(c => c.Id);
            var _cakebase = await _context.CakeBases.ToListAsync();
            Dictionary<int, CakeBasis> cakebaseMap = new Dictionary<int, CakeBasis>();
            cakebaseMap = _cakebase.ToDictionary(c => c.Id);
            Dictionary<int, List<CakeBasis>> sizeCakeBaseMap = new Dictionary<int, List<CakeBasis>>();
            List<PizzaViewModel> pizzaViewModels = new List<PizzaViewModel>();
            foreach (var pizza in _pizzas)
            {
                List<CakeBasis> cakeBases = new List<CakeBasis>();
                PizzaViewModel pizzaViewModel = new PizzaViewModel();
                pizzaViewModel.Id = pizza.Id;
                pizzaViewModel.Name = pizza.Name;
                pizzaViewModel.Price = pizza.Price;
                pizzaViewModel.Description = pizza.Description;
                pizzaViewModel.Sizes = new List<Size>();
                var SizeIds = pizza.PizzaOptions.Select(x => x.SizeId).Distinct().ToList();
                foreach (var sizeId in SizeIds)
                {
                    if (sizeMap.TryGetValue(sizeId, out var size))
                    {
                        pizzaViewModel.Sizes.Add(size);
                        var _CakeBases = pizza.PizzaOptions.Where(X => X.SizeId == sizeId).Select(x => x.CakeBaseId).Distinct().ToList();
                        foreach (var cakeBaseId in _CakeBases)
                        {
                            if (cakebaseMap.TryGetValue((int)cakeBaseId, out CakeBasis cakeBasis))
                            {
                                cakeBases.Add(cakeBasis);
                            }
                        }
                        sizeCakeBaseMap[sizeId] = cakeBases;
                        pizzaViewModel.sizeCakeBase = sizeCakeBaseMap;
                    }
                }
                pizzaViewModels.Add(pizzaViewModel);
            }
            _pizzaViewModels = pizzaViewModels;
        }
    }
}
