using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Web_PizzaShop.Interface.Admin;
using Web_PizzaShop.Models;

namespace Web_PizzaShop.Pages.Admin
{
    public class OrderListModel : PageModel
    {
        private readonly ILogger<ProductListModel> logger;
        private readonly IAdminService service;
        private readonly PRN221_PRJContext context;
        public List<Order>? orders { get; set; }
        public List<Category>? categories { get; set; }
        public int countPages { get; set; }
        [BindProperty(SupportsGet = true, Name = "pageNumber")]
        public int currentPage { get; set; }
        public int item_per_page = 5;
        public int total_orders { get; set; }
        public OrderListModel(ILogger<ProductListModel> logger, IAdminService service, PRN221_PRJContext context)
        {
            this.logger = logger;
            this.service = service;
            this.context = context;
        }

        public async Task OnGet(string OrderId, string CustomerName, string Email, string Status, string Total,
        DateTime FromDate, DateTime ToDate)
        {
            if (string.IsNullOrEmpty(OrderId) && string.IsNullOrEmpty(CustomerName) && string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Status) && string.IsNullOrEmpty(Total)
            && FromDate == default && ToDate == default)
            {
                total_orders = context.Orders.Count();
                countPages = (int)Math.Ceiling((double)total_orders / item_per_page);
                if (currentPage < 1)
                {
                    currentPage = 1;
                }
                if (currentPage > countPages)
                {
                    currentPage = countPages;
                }
                orders = await service.GetAllOrder(currentPage, item_per_page);
            }
            else
            {
                total_orders = await service.FilterOrderCount(OrderId, CustomerName, Email, Status, Total, FromDate.ToString(), ToDate.ToString());
                countPages = (int)Math.Ceiling((double)total_orders / item_per_page);
                if (currentPage < 1)
                {
                    currentPage = 1;
                }
                if (currentPage > countPages)
                {
                    currentPage = countPages;
                }
                orders = await service.FilterOrder(OrderId, CustomerName, Email, Status, Total, FromDate.ToString(), ToDate.ToString(), currentPage, item_per_page);
            }
        }

        public async Task<IActionResult> OnPostDeleteOrder()
        {
            string Ids = Request.Form["sizeId"];
            var selectedIds = Ids.Split(',').Select(int.Parse).ToList();
            var PizzaOrdertoDelete = await context.PizzaOrders.Where(po => selectedIds.Contains(po.OrderId)).ToListAsync();
            var OrderToDelete = await context.Orders.Where(o => selectedIds.Contains(o.OrderId)).ToListAsync();
            foreach (var po in PizzaOrdertoDelete)
            {
                context.PizzaOrders.Remove(po);
            }

            foreach (var o in OrderToDelete)
            {
                context.Orders.Remove(o);
            }
            await context.SaveChangesAsync();
            total_orders = context.Orders.Count();
            countPages = (int)Math.Ceiling((double)total_orders / item_per_page);
            if (currentPage < 1)
            {
                currentPage = 1;
            }
            if (currentPage > countPages)
            {
                currentPage = countPages;
            }
            List<Order> _orders = await service.GetAllOrder(currentPage, item_per_page);
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };
            return new JsonResult(_orders, jsonSerializerOptions);
        }
    }
}
