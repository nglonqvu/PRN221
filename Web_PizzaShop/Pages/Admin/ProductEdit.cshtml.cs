using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Web_PizzaShop.Hubs;
using Web_PizzaShop.Interface.Admin;
using Web_PizzaShop.Models;

namespace Web_PizzaShop.Pages.Admin
{
    public class ProductEditModel : PageModel
    {
        IWebHostEnvironment webHostEnvironment;
        private readonly ILogger<ProductEditModel> logger;
        private readonly IAdminService service;
        private readonly PRN221_PRJContext context;
        private readonly IHubContext<HubService> _hubContext;
        [BindProperty]
        public Pizza pizza { get; set; }
        public List<Category>? categories { get; set; }
        public ProductEditModel(IWebHostEnvironment webHostEnvironment, ILogger<ProductEditModel> logger, IAdminService service, PRN221_PRJContext context, IHubContext<HubService> hubContext)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.logger = logger;
            this.service = service;
            this.context = context;
            _hubContext = hubContext;
        }
        public async Task OnGet(int itemid)
        {
            pizza = await service.GetPizzaById(itemid);
            categories = await service.GetAllCategory();
        }

        public async Task OnPost()
        {
            int selectedCategoryId = int.Parse(Request.Form["CategoryId"]);
            bool success = await service.UpdatePizza(pizza, selectedCategoryId);
            if (success)
            {
                pizza = await service.GetPizzaById(pizza.Id);
                categories = await service.GetAllCategory();
                await _hubContext.Clients.All.SendAsync("ReloadData");
                TempData["msg"] = "Your changes have been saved successfully.";
            }
            else
            {
                TempData["msg"] = "Edit fail!!";
            }
        }

        public async Task OnPostUploadFile(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var filePath = Path.Combine(webHostEnvironment.WebRootPath + "/images/pizzas", file.FileName);
                Console.WriteLine(filePath);
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