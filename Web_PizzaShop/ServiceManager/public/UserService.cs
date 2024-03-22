using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web_PizzaShop.Interface.Public;
using Web_PizzaShop.Models;

namespace Web_PizzaShop.ServiceManager
{


    public class UserService : IUserService
    {
        private readonly PRN221_PRJContext _context;
        public UserService(PRN221_PRJContext context)
        {
            _context = context;
        }
       
        public async Task<User?> Login(User user)
        {

            var userLoged = await _context.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Email == user.Email);
            
            if (!BCrypt.Net.BCrypt.Verify(user.PasswordHash, userLoged.PasswordHash))
            {
                return null;
            }
            return userLoged;
        }
        public async Task<User> FindExistEmail(string email)
        {
            var userLoged = await _context.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
            return userLoged;
        }
        public async Task<bool> Regis(User user)
        {
            var check = FindExistEmail(user.Email);
            if (check == null)
            {
                User newUser = new User()
                {
                    Email = user.Email,
                    PasswordHash = user.PasswordHash,
                    UserName = user.UserName,
                };
                await _context.AddAsync(newUser);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}


