﻿using Web_PizzaShop.Models;

namespace Web_PizzaShop.Interface.Public
{
    public interface IUserService
    {
        Task<User> Login(User user);
        Task<Tuple<bool, int>> Regis(User user);
        Task<string> GetUserRoleByUserId(int userId);
    }
}
