using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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
        public string Message { get; set; }
        public async Task OnGetAsync([FromQuery] int userId)
        {
            var user = await _dbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
            user.IsActive = true;
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
            Message = "Register Successfully!";
        }
    }
}
