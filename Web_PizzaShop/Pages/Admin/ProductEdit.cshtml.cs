using System.Text.Json;
using System.Text.Json.Serialization;
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
        public List<Size> sizes { get; set; }
        public List<Size> pizza_sizes { get; set; }
        public List<CakeBasis> cakeBases { get; set; }
        public ProductEditModel(IWebHostEnvironment webHostEnvironment, ILogger<ProductEditModel> logger, IAdminService service, PRN221_PRJContext context, IHubContext<HubService> hubContext)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.logger = logger;
            this.service = service;
            this.context = context;
            _hubContext = hubContext;
        }
        public async Task<IActionResult> OnGet(int itemid)
        {
            string userRole = HttpContext.Session.GetString("userRole");
            if (string.IsNullOrEmpty(userRole) || !userRole.Equals("Admin"))
            {
                return RedirectToPage("../Common/AuthorFailed");
            }
            pizza = await service.GetPizzaById(itemid);
            categories = await service.GetAllCategory();
            sizes = await service.GetSizes();
            pizza_sizes = await service.GetSizesByPizzaId(itemid);
            cakeBases = context.CakeBases.ToList();
            return Page();
        }

        public async Task OnPost()
        {
            int selectedCategoryId = int.Parse(Request.Form["CategoryId"]);
            bool success = await service.UpdatePizza(pizza, selectedCategoryId);
            if (success)
            {
                await OnGet(pizza.Id);
                await _hubContext.Clients.All.SendAsync("ReloadData");
                TempData["msg"] = "Your changes have been saved successfully.";
            }
            else
            {
                TempData["msg"] = "Edit fail!!";
            }
        }

        public async Task OnPostAddSize()
        {
            int size = int.Parse(Request.Form["selectedSize"]);
            sizes = await service.GetSizesByPizzaId(pizza.Id);
            foreach (var size1 in sizes)
            {
                if (size == size1.Id)
                {
                    await OnGet(pizza.Id);
                    TempData["msg"] = "Size already exist";
                    break;
                }
            }
            if (TempData["msg"] == null)
            {
                bool success = await service.AddPizzaSize(pizza.Id, size);
                if (success)
                {
                    await OnGet(pizza.Id);
                    await _hubContext.Clients.All.SendAsync("ReloadData");
                    TempData["msg"] = "Add size successfully.";
                }
                else
                {
                    await OnGet(pizza.Id);
                    TempData["msg"] = "Add size fail!!";
                }
            }
        }

        public async Task OnPostDeleteSize()
        {
            int size = int.Parse(Request.Form["SizeIdDelete"]);
            var PizzaDelete = await context.PizzaOptions.Where(po => po.SizeId == size && po.PizzaId == pizza.Id).ToListAsync();
            foreach (var Pizza in PizzaDelete)
            {
                context.PizzaOptions.Remove(Pizza);
            }
            await context.SaveChangesAsync();
            await OnGet(pizza.Id);
            await _hubContext.Clients.All.SendAsync("ReloadData");
            TempData["msg"] = "Delete size successfully.";
        }

        public async Task<IActionResult> OnPostCakeBaseByPizza_Size()
        {
            if (int.TryParse(Request.Form["sizeId"], out int sizeId) && int.TryParse(Request.Form["pizzaId"], out int pizzaId))
            {
                List<CakeBasis> cakeBases = await service.GetCakeBasisBySizePizza(pizzaId, sizeId);

                var jsonSerializerOptions = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                };
                return new JsonResult(cakeBases, jsonSerializerOptions);
            }
            return BadRequest("Invalid sizeId");
        }

        public async Task<IActionResult> OnPostAddCakeBase()
        {
            if (int.TryParse(Request.Form["sizeId"], out int sizeId) && int.TryParse(Request.Form["pizzaId"], out int pizzaId) && int.TryParse(Request.Form["cakebaseId"], out int cakebaseId))
            {
                if (await service.AddCakeBase(pizzaId, sizeId, cakebaseId))
                {
                    List<CakeBasis> cakeBases = await service.GetCakeBasisBySizePizza(pizzaId, sizeId);

                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.Preserve,
                    };
                    return new JsonResult(cakeBases, jsonSerializerOptions);
                }
                else
                {
                    return BadRequest("Invalid sizeId");
                }
            }
            return BadRequest("Invalid sizeId");
        }

        public async Task<IActionResult> OnPostDeleteCakeBase()
        {
            if (int.TryParse(Request.Form["sizeId"], out int sizeId) && int.TryParse(Request.Form["pizzaId"], out int pizzaId) && int.TryParse(Request.Form["optionId"], out int optionId))
            {
                PizzaOption? po = await context.PizzaOptions.FirstOrDefaultAsync(po => po.OptionId == optionId);
                context.PizzaOptions.Remove(po);
                await context.SaveChangesAsync();
                List<CakeBasis> cakeBases = await service.GetCakeBasisBySizePizza(pizzaId, sizeId);
                var jsonSerializerOptions = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                };
                return new JsonResult(cakeBases, jsonSerializerOptions);
            }
            return BadRequest("Invalid optionId");
        }
    
        public async Task OnPostUploadFile(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                byte[] imageData;
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    imageData = stream.ToArray();
                }
                var base64String = Convert.ToBase64String(imageData);
                var _pizza = await service.GetPizzaById(pizza.Id);
                _pizza.ImageUrl = base64String;
                await context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("ReloadData");
                await OnGet(pizza.Id);
            }
        }
    }
}
