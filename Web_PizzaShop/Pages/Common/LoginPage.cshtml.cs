using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Web_PizzaShop.Interface.Public;
using Web_PizzaShop.Models;

namespace Web_PizzaShop.Pages.Common
{
    public class LoginPageModel : PageModel
    {

        private readonly ILogger<IndexModel> _logger;
        private readonly IUserService _userService;
        private readonly PRN221_PRJContext _context;
        public LoginPageModel(ILogger<IndexModel> logger, IUserService userService, PRN221_PRJContext context)
        {
            _logger = logger;
            _userService = userService;
            _context = context;
        }

        [BindProperty]
        public String email { get; set; }

        [BindProperty]
        public String password { get; set; }
        [BindProperty]
        public String msg {  get; set; }
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
                    Email = email,
                    PasswordHash = password
                };

                User user = await _userService.Login(userLogin);
                if (user != null)
                {
<<<<<<< Updated upstream
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.IgnoreCycles,
                        WriteIndented = true
                    };

                    HttpContext.Session.SetString("account", JsonSerializer.Serialize(user, jsonSerializerOptions));
                    List<Role> userrole = user.Roles.ToList();
                    string role = userrole[0].Name;
                    if (role == "Admin")
                    {
                        return RedirectToPage("../Admin/Dashboard");
                    }
                    else
                    {
                        return RedirectToPage("../Index");
                    }
=======
                    var userRole = await _userService.GetUserRoleByUserId(user.Id);
                    if (string.IsNullOrEmpty(userRole))
                    {
                        return Page();
                    }
                    HttpContext.Session.SetString("userRole", user.Id.ToString());
                    HttpContext.Session.SetString("username", userName);
                    //return NotFound();
                    return RedirectToPage("../Index");
>>>>>>> Stashed changes
                }
                
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}
