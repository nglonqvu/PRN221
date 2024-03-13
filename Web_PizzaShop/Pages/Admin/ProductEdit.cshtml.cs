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
        public async Task OnGet(int itemid)
        {
            pizza = await service.GetPizzaById(itemid);
            categories = await service.GetAllCategory();
            sizes = await service.GetSizes();
            pizza_sizes = await service.GetSizesByPizzaId(itemid);
            cakeBases = context.CakeBases.ToList();
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

        public async Task<IActionResult> OnPostAddSize()
        {
            if (int.TryParse(Request.Form["sizeId"], out int sizeId))
            {
                sizes = await service.GetSizesByPizzaId(pizza.Id);
                foreach (var size1 in sizes)
                {
                    if (sizeId == size1.Id)
                    {
                        return BadRequest("Invalid sizeId");
                    }
                }
                if (await service.AddPizzaSize(pizza.Id, sizeId))
                {
                    List<Size> _sizes = await service.GetSizesByPizzaId(pizza.Id);

                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.Preserve,
                    };
                    return new JsonResult(_sizes, jsonSerializerOptions);
                }
                else
                {
                    return BadRequest("Invalid sizeId");
                }
            }
            return BadRequest("Invalid sizeId");
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
    }
}
