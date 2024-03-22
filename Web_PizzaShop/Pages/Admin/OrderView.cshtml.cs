using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web_PizzaShop.Interface.Admin;
using Web_PizzaShop.Models;

namespace Web_PizzaShop.Pages.Admin
{
    public class OrderViewModel : PageModel
    {
        private readonly ILogger<ProductListModel> logger;
        private readonly IAdminService service;
        private readonly PRN221_PRJContext context;
        public Order? order { get; set; }
        public List<PizzaOrder>? pizzaOrders { get; set; }
        public OrderViewModel(ILogger<ProductListModel> logger, IAdminService service, PRN221_PRJContext context)
        {
            this.logger = logger;
            this.service = service;
            this.context = context;
        }
        public async Task<IActionResult> OnGet(int itemid)
        {
            string userRole = HttpContext.Session.GetString("userRole");
            if (string.IsNullOrEmpty(userRole) || !userRole.Equals("Admin"))
            {
                return RedirectToPage("../Common/AuthorFailed");
            }
            order = await service.GetOrderByOrderId(itemid);
            pizzaOrders = await service.GetListPizzaOrder(itemid);
            return Page();
        }
    }
}
