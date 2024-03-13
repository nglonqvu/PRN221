using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web_PizzaShop.Models;

namespace Web_PizzaShop.Pages.Common
{
    public class MailConfirmModel : PageModel
    {
        private readonly PRN221_PRJContext _dbContext;
        public MailConfirmModel(PRN221_PRJContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> OnGetAsync(string token)
        {
            if (TempData["ConfirmToken"] !=null && TempData["ConfirmToken"].ToString() == token)
            {
                var  ConfirmExpired = DateTime.UtcNow.AddMinutes(1);
                if(DateTime.Now <= ConfirmExpired)
                {
                    var Email = TempData["Email"].ToString();
                    var Username = TempData["Username"].ToString();
                    var PhoneNumber = TempData["PhoneNumber"].ToString();
                    var PasswordHash = TempData["PasswordHash"].ToString();
                    var CreatedAt = DateTime.UtcNow;

                    var user = new User
                    {
                        Email = Email,
                        UserName = Username,
                        PhoneNumber = PhoneNumber,
                        CreatedAt = CreatedAt,
                        PasswordHash = PasswordHash
                    };
                    _dbContext.Users.Add(user);
                    _dbContext.SaveChanges();
                    TempData["Message"] = "Tài kho?n ?ã ???c kích ho?t thành công.";
                    return RedirectToPage("/Account/Login");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Quá th?i gian kích ho?t. Vui lòng th? l?i.";
                return RedirectToPage("/TimeOut");
            }
            TempData["ErrorMessage"] = "Liên k?t kích ho?t không h?p l?.";
            return RedirectToPage("/Error");
        }
    }
}
