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
        Task<List<Size>> GetSizes();
        Task<List<Size>> GetSizesByPizzaId(int pizzaid);
        Task<bool> AddPizzaSize(int pizzaId, int sizeId);
        Task<List<CakeBasis>> GetCakeBasisBySizePizza(int pid, int sid);
        Task<bool> AddCakeBase(int pid, int sid, int cbid);
        Task<List<Order>> GetAllOrder(int currentPage, int item_per_page);
        Task<int> FilterOrderCount(string OrderId, string CustomerName, string Email, string Status, string Total,
        string FromDate, string ToDate);
        Task<List<Order>> FilterOrder(string OrderId, string CustomerName, string Email, string Status, string Total,
        string FromDate, string ToDate, int currentPage, int itemsPerPage);
        Task<Order> GetOrderByOrderId(int orderId);
        Task<List<PizzaOrder>> GetListPizzaOrder(int orderId);
    }
}
