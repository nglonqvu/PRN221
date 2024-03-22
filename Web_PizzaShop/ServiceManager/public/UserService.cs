using BCrypt.Net;
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
        public async Task<User> FindExistEmailAndUserName(string email, string username)
        {
            var userLoged = await _context.Users.Where(x => x.Email == email || x.UserName == username).FirstOrDefaultAsync();
            return userLoged;
        }
        public async Task<Tuple<bool, int>> Regis(User user)
        {
            var userExist = await FindExistEmailAndUserName(user.Email, user.UserName);
            if (userExist == null)
            {
                User newUser = new User()
                {
                    Email = user.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash),
                    UserName = user.UserName,
                    Address = user.Address,
                };
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                Tuple<bool, int> result = new Tuple<bool, int>(true, newUser.Id);
                return result;
            }
            else
            {
                return new Tuple<bool, int>(false, 0);
            }
        }
        public async Task<string> GetUserRoleByUserId(int userId)
        {
            var query = @$"
                SELECT Roles.*
                FROM Users
                INNER JOIN UserRoles ON Users.Id = UserRoles.UserId
                INNER JOIN Roles ON UserRoles.RoleId = Roles.Id
                WHERE Users.Id = {userId}";
            var userRole = await _context.Roles
            .FromSqlRaw(query)
            .AsNoTracking()
    .FirstOrDefaultAsync();
            //var userRole = await _context.Roles.Include(x => x.Users).Where(x => x.Users.Select(x => x.Id).ToList().Contains(userId)).FirstOrDefaultAsync();
            if (userRole == null)
            {
                return "";
            }
            else
            {
                return userRole.Name;
            }
        }
    }
}


