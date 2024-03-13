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
                    TempData["Message"] = "T�i kho?n ?� ???c k�ch ho?t th�nh c�ng.";
                    return RedirectToPage("/Account/Login");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Qu� th?i gian k�ch ho?t. Vui l�ng th? l?i.";
                return RedirectToPage("/TimeOut");
            }
            TempData["ErrorMessage"] = "Li�n k?t k�ch ho?t kh�ng h?p l?.";
            return RedirectToPage("/Error");
        }
    }
}
