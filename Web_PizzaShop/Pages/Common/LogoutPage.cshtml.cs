using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Web_PizzaShop.Interface.Public;
using Web_PizzaShop.Models;

namespace Web_PizzaShop.Pages.Common
{
    public class LogoutPageModel : PageModel
    {

        private readonly ILogger<IndexModel> _logger;
        private readonly IUserService _userService;
        private readonly PRN221_PRJContext _context;
        public LogoutPageModel(ILogger<IndexModel> logger, IUserService userService, PRN221_PRJContext context)
        {
            _logger = logger;
            _userService = userService;
            _context = context;
        }

        public IActionResult OnPostLogout(){
            HttpContext.Session.Clear();
            return RedirectToPage("/Common/LoginPage");
        }
    }
}
