using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_PizzaShop.Interface.Admin;
using Web_PizzaShop.Models;

namespace Web_PizzaShop.Pages.Admin
{
    public class ProductListModel : PageModel
    {
        private readonly ILogger<ProductListModel> logger;
        private readonly IAdminService service;
        private readonly PRN221_PRJContext context;
        public List<Pizza>? pizzas { get; set; }
        public List<Category>? categories { get; set; }
        public int countPages { get; set; }
        [BindProperty(SupportsGet = true, Name = "pageNumber")]
        public int currentPage { get; set; }
        public int item_per_page = 5;
        public int total_pizzas { get; set; }

        public ProductListModel(ILogger<ProductListModel> logger, IAdminService service, PRN221_PRJContext context)
        {
            this.logger = logger;
            this.service = service;
            this.context = context;
        }

        public async Task OnGet(string PizzaName, string Description, string Price, string Hot, string Category, DateTime DateCreate, DateTime DateDelete)
        {
            if (string.IsNullOrEmpty(PizzaName) && string.IsNullOrEmpty(Description) && string.IsNullOrEmpty(Price) && string.IsNullOrEmpty(Hot) && string.IsNullOrEmpty(Category) && DateCreate == default && DateDelete == default)
            {
                await LoadDataAsync();
            }
            else
            {
                total_pizzas = await service.FilterPizzaCount(PizzaName, Description, Price, Hot, Category, DateCreate.ToString(), DateDelete.ToString());
                countPages = (int)Math.Ceiling((double)total_pizzas / item_per_page);
                if (currentPage < 1)
                {
                    currentPage = 1;
                }
                if (currentPage > countPages)
                {
                    currentPage = countPages;
                }
                pizzas = await service.FilterPizza(PizzaName, Description, Price, Hot, Category, DateCreate.ToString(), DateDelete.ToString(), currentPage, item_per_page);
            }
            categories = await service.GetAllCategory();
        }


        private async Task LoadDataAsync()
        {
            total_pizzas = await context.Pizzas.CountAsync();
            countPages = (int)Math.Ceiling((double)total_pizzas / item_per_page);
            if (currentPage < 1)
            {
                currentPage = 1;
            }
            if (currentPage > countPages)
            {
                currentPage = countPages;
            }
            pizzas = await service.GetAllPizza(currentPage, item_per_page);
        }

        public async Task OnPostDeletePizzas()
        {
            string _selectedIds = Request.Form["selectedPizzaIds"];
            var selectedIds = _selectedIds.Split(',').Select(int.Parse).ToList();
            var optiontoDelete = await context.PizzaOptions.Where(p => selectedIds.Contains(p.PizzaId)).ToListAsync();
            var pizzasToDelete = await context.Pizzas.Where(p => selectedIds.Contains(p.Id)).ToListAsync();
            foreach (var option in optiontoDelete)
            {
                context.PizzaOptions.Remove(option);
            }

            foreach (var pizza in pizzasToDelete)
            {
                context.Pizzas.Remove(pizza);
            }
            await context.SaveChangesAsync();
            await LoadDataAsync();
            categories = await service.GetAllCategory();
            TempData["msg"] = "Delete pizza successfully !!";
        }
    }
}
