using Web_PizzaShop.Models;

namespace Web_PizzaShop.Interface.Admin
{
    public interface IAdminService
    {
        Task<List<Pizza>> GetAllPizza(int currentPage, int item_per_page);
        Task<int> FilterPizzaCount(string pizzaName, string description, string price, string isHot, string category,
        string dateCreate, string dateDelete);
        Task<List<Pizza>> FilterPizza(string pizzaName, string description, string price, 
        string isHot, string category, string dateCreate, string dateDelete, int currentPage, int itemsPerPage);
        Task<List<Category>> GetAllCategory();
        Task<Pizza> GetPizzaById(int id);
        Task<bool> UpdatePizza(Pizza pizza, int categoryid);
        Task<bool> AddPizza(Pizza pizza, int categoryId);
    }
}
