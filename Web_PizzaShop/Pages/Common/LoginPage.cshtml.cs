using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web_PizzaShop.Interface.Public;
using Web_PizzaShop.Models;

namespace Web_PizzaShop.Pages.Common
{
    public class LoginPageModel : PageModel
    {

        private readonly ILogger<IndexModel> _logger;
        private readonly IUserService _userService;

        [BindProperty]
        public User UserLogin { get; set; }
        public void OnGet()
        {

        }
        public IActionResult OnPost(User userLogin)
        {
            try
            {
                userLogin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userLogin.PasswordHash);
                _logger.LogError(userLogin.PasswordHash);
                var user = _userService.Login(userLogin).Result;
                if (user != null)
                {
                    return RedirectToPage("./");
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;

            }
        }
    }
}
