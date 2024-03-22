using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Web_PizzaShop.Hubs;
using Web_PizzaShop.Interface.Admin;
using Web_PizzaShop.Models;

namespace Web_PizzaShop.Pages.Admin
{
    public class AddProductModel : PageModel
    {
        IWebHostEnvironment webHostEnvironment;
        private readonly ILogger<AddProductModel> logger;
        private readonly IAdminService service;
        private readonly PRN221_PRJContext context;
        private readonly IHubContext<HubService> _hubContext;
        [BindProperty]
        public Pizza pizza { get; set; }
        public List<Category> categories { get; set; }
        public AddProductModel(IWebHostEnvironment webHostEnvironment, ILogger<AddProductModel> logger, IAdminService service, PRN221_PRJContext context, IHubContext<HubService> hubContext)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.logger = logger;
            this.service = service;
            this.context = context;
            _hubContext = hubContext;
        }
        public async Task<IActionResult> OnGet()
        {
            string userRole = HttpContext.Session.GetString("userRole");
            if (string.IsNullOrEmpty(userRole) || !userRole.Equals("Admin"))
            {
                return RedirectToPage("../Common/AuthorFailed");
            }
            categories = await service.GetAllCategory();
            return Page();
        }

        public async Task OnPost()
        {
            int selectedCategoryId = int.Parse(Request.Form["CategoryId"]);
            List<Pizza> pizzas = await context.Pizzas.ToListAsync();
            foreach (var _pizza in pizzas)
            {
                if (pizza.Name == _pizza.Name)
                {
                    TempData["msg"] = "Pizza Name already exist!!";
                    break;
                }
            }
            bool success = await service.AddPizza(pizza, selectedCategoryId);
            if (success)
            {
                pizza = await service.GetPizzaById(pizza.Id);
                categories = await service.GetAllCategory();
                await _hubContext.Clients.All.SendAsync("ReloadData");
                TempData["msg"] = "Add pizza successfully !!";
            }
            else
            {
                TempData["msg"] = "Add fail !!";
            }
        }

        public async Task OnPostUploadFile(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var filePath = Path.Combine(webHostEnvironment.WebRootPath + "/images/pizzas", file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            pizza = await service.GetPizzaById(pizza.Id);
            pizza.ImageUrl = file.FileName;
            await context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReloadData");
            categories = await service.GetAllCategory();
        }
    }
}
