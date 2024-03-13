using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web_PizzaShop.Models;


namespace Web_PizzaShop.Pages.Common
{
    public class LoginModel : PageModel
    {
        private readonly PRN221_PRJContext _context;
        public LoginModel(PRN221_PRJContext context)
        {
            _context = context;
        }
        public String message;
        public void OnGet()
        {
        }

        public IActionResult OnPost(String email, String password) 
        {
            var hashedPassword = "";
            Models.User user = _context.Users.FirstOrDefault(x => x.Email == email && x.PasswordHash == hashedPassword);
            if (_context.Users.FirstOrDefault(x => x.Email == email && x.PasswordHash == hashedPassword) != null)
            {
            }
            else
            {
                message = "Login Failed";
                return Page();
            }
            return null;
        }
    }
}
