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
        public LoginPageModel(ILogger<IndexModel> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [BindProperty]
        public String userName { get; set; }

        [BindProperty]
        public String password { get; set; }
        public IActionResult OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            try
            {
                User userLogin = new User
                {
                    UserName = userName,
                    PasswordHash = password
                };
               
                var user = _userService.Login(userLogin).Result;
                if (user != null)
                {
                    //return NotFound();
                    return RedirectToPage("../Index");
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
