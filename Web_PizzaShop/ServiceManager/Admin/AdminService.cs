using Microsoft.EntityFrameworkCore;
using Web_PizzaShop.Interface.Admin;
using Web_PizzaShop.Models;

namespace Web_PizzaShop.ServiceManager.Admin
{
    public class AdminService : IAdminService
    {
        private readonly PRN221_PRJContext _dbContext;

        public AdminService(PRN221_PRJContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Pizza>> GetAllPizza(int currentPage, int item_per_page)
        {
            List<Category> categories = await GetAllCategory();
            var categoryMap = categories.ToDictionary(c => c.Id);

            var query = _dbContext.Pizzas.OrderByDescending(pizza => pizza.CreatedAt).Skip((currentPage - 1) * item_per_page)
                                          .Take(item_per_page);

            List<Pizza> pizzas = await query.ToListAsync();

            foreach (var pizza in pizzas)
            {
                if (categoryMap.TryGetValue(pizza.CategoriesId, out var category))
                {
                    pizza.Categories = category;
                }
            }

            return pizzas;
        }

        public async Task<int> FilterPizzaCount(string pizzaName, string description, string price, string isHot, string category,
        string dateCreate, string dateDelete)
        {
            var query = _dbContext.Pizzas.AsQueryable();

            if (!string.IsNullOrEmpty(pizzaName))
            {
                query = query.Where(p => p.Name.Contains(pizzaName));
            }

            if (!string.IsNullOrEmpty(description))
            {
                query = query.Where(p => p.Description.Contains(description));
            }

            if (!string.IsNullOrEmpty(price))
            {
                if (decimal.TryParse(price, out decimal parsedPrice))
                {
                    query = query.Where(p => p.Price == parsedPrice);
                }
            }

            if (!string.IsNullOrEmpty(isHot))
            {
                if (bool.TryParse(isHot, out bool parsedIsHot))
                {
                    query = query.Where(p => p.IsPizzaOfTheWeek == parsedIsHot);
                }
            }

            if (!string.IsNullOrEmpty(category))
            {
                if (int.TryParse(category, out int parsedCategoryId))
                {
                    query = query.Where(p => p.CategoriesId == parsedCategoryId);
                }
            }

            if (!string.IsNullOrEmpty(dateCreate))
            {
                if (DateTime.TryParse(dateCreate, out DateTime parsedDateCreate))
                {
                    if (parsedDateCreate != default)
                    {
                        query = query.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date == parsedDateCreate.Date);
                    }
                }
            }

            if (!string.IsNullOrEmpty(dateDelete))
            {
                if (DateTime.TryParse(dateDelete, out DateTime parsedDateDelete))
                {
                    if (parsedDateDelete != default)
                    {
                        query = query.Where(p => p.DeletedAt.HasValue && p.DeletedAt.Value.Date == parsedDateDelete.Date);
                    }
                }
            }
            var filteredPizzas = await query.OrderByDescending(pizza => pizza.CreatedAt).ToListAsync();
            return filteredPizzas.Count();
        }

        public async Task<List<Pizza>> FilterPizza(string pizzaName, string description, string price,
        string isHot, string category, string dateCreate, string dateDelete, int currentPage, int itemsPerPage)
        {
            var query = _dbContext.Pizzas.AsQueryable();

            if (!string.IsNullOrEmpty(pizzaName))
            {
                query = query.Where(p => p.Name.Contains(pizzaName));
            }

            if (!string.IsNullOrEmpty(description))
            {
                query = query.Where(p => p.Description.Contains(description));
            }

            if (!string.IsNullOrEmpty(price))
            {
                if (decimal.TryParse(price, out decimal parsedPrice))
                {
                    query = query.Where(p => p.Price == parsedPrice);
                }
            }

            if (!string.IsNullOrEmpty(isHot))
            {
                if (bool.TryParse(isHot, out bool parsedIsHot))
                {
                    query = query.Where(p => p.IsPizzaOfTheWeek == parsedIsHot);
                }
            }

            if (!string.IsNullOrEmpty(category))
            {
                if (int.TryParse(category, out int parsedCategoryId))
                {
                    query = query.Where(p => p.CategoriesId == parsedCategoryId);
                }
            }

            if (!string.IsNullOrEmpty(dateCreate))
            {
                if (DateTime.TryParse(dateCreate, out DateTime parsedDateCreate))
                {
                    if (parsedDateCreate != default)
                    {
                        query = query.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date == parsedDateCreate.Date);
                    }
                }
            }

            if (!string.IsNullOrEmpty(dateDelete))
            {
                if (DateTime.TryParse(dateDelete, out DateTime parsedDateDelete))
                {
                    if (parsedDateDelete != default)
                    {
                        query = query.Where(p => p.DeletedAt.HasValue && p.DeletedAt.Value.Date == parsedDateDelete.Date);
                    }
                }
            }
            currentPage = currentPage <= 0 ? 1 : currentPage;
            query = query.Skip((currentPage - 1) * itemsPerPage)
                         .Take(itemsPerPage);

            List<Pizza> filteredPizzas = await query.OrderByDescending(pizza => pizza.CreatedAt).ToListAsync();
            List<Category> categories = await GetAllCategory();
            var categoryMap = categories.ToDictionary(c => c.Id);
            foreach (var pizza in filteredPizzas)
            {
                if (categoryMap.TryGetValue(pizza.CategoriesId, out var _category))
                {
                    pizza.Categories = _category;
                }
            }
            return filteredPizzas;
        }


        public async Task<List<Category>> GetAllCategory()
        {
            List<Category> categories = await _dbContext.Categories.OrderBy(cate => cate.Id).ToListAsync();
            return categories;
        }

        public async Task<Pizza> GetPizzaById(int id)
        {
            Pizza? pizza = await _dbContext.Pizzas.Where(p => p.Id == id).FirstOrDefaultAsync();
            return pizza;
        }

        public async Task<bool> UpdatePizza(Pizza pizza, int categoryId)
        {
            var existingPizza = await _dbContext.Pizzas.FindAsync(pizza.Id);
            if (existingPizza == null)
            {
                return false;
            }

            existingPizza.Name = pizza.Name;
            existingPizza.Price = pizza.Price;
            existingPizza.Description = pizza.Description;
            existingPizza.IsPizzaOfTheWeek = pizza.IsPizzaOfTheWeek;

            var category = await _dbContext.Categories
                .Where(cate => cate.Id == categoryId)
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return false;
            }

            existingPizza.Categories = category;
            existingPizza.CategoriesId = categoryId;
            _dbContext.Entry(existingPizza).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return true;
        }


        public async Task<bool> AddPizza(Pizza pizza, int categoryId)
        {
            try
            {
                Pizza _pizza = new Pizza
                {
                    Name = pizza.Name,
                    Price = pizza.Price,
                    Description = pizza.Description,
                    IsPizzaOfTheWeek = pizza.IsPizzaOfTheWeek,
                    CategoriesId = categoryId,
                    CreatedAt = DateTime.Now,
                    DeletedAt = null
                };

                var category = await _dbContext.Categories
                    .Where(cate => cate.Id == categoryId)
                    .FirstOrDefaultAsync();

                if (category == null)
                {
                    return false;
                }

                _pizza.Categories = category;
                _dbContext.Pizzas.Add(_pizza);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<Size>> GetSizes()
        {
            List<Size> sizes = await _dbContext.Sizes.ToListAsync();
            return sizes;
        }

        public async Task<List<Size>> GetSizesByPizzaId(int pizzaid)
        {
            List<PizzaOption> pizzaOptions = await _dbContext.PizzaOptions.Where(pro => pro.PizzaId == pizzaid).OrderBy(pro => pro.SizeId).ToListAsync();
            List<Size> sizes = await GetSizes();
            List<Size> sizepiiza = new List<Size>();
            var sizeMap = sizes.ToDictionary(c => c.Id);
            foreach (var pizza in pizzaOptions)
            {
                if (sizeMap.TryGetValue(pizza.SizeId, out var _size))
                {
                    sizepiiza.Add(_size);
                }
            }
            sizepiiza = sizepiiza.Distinct().ToList();
            return sizepiiza;
        }

        public async Task<bool> AddPizzaSize(int pizzaId, int sizeId)
        {
            try
            {
                List<CakeBasis> cakeBases = _dbContext.CakeBases.ToList();
                int randomIndex = new Random().Next(cakeBases.Count);
                CakeBasis cakeBasis = cakeBases[randomIndex];
                PizzaOption pizzaOption = new PizzaOption()
                {
                    CakeBaseId = cakeBasis.Id,
                    PizzaId = pizzaId,
                    SizeId = sizeId
                };
                _dbContext.PizzaOptions.Add(pizzaOption);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<CakeBasis>> GetCakeBasisBySizePizza(int pid, int sid)
        {
            List<int> CakeBaseIds = new List<int>();
            List<PizzaOption> pizzaOptions = await _dbContext.PizzaOptions.Where(po => po.PizzaId == pid && po.SizeId == sid).ToListAsync();
            if (pizzaOptions == null)
            {
                return null;
            }
            foreach (var PizzaOption in pizzaOptions)
            {
                CakeBaseIds.Add(PizzaOption.CakeBaseId);
            }
            List<CakeBasis> selectedProducts = await _dbContext.CakeBases
            .Where(cb => CakeBaseIds.Contains(cb.Id))
            .ToListAsync();
            return selectedProducts;
        }

        public async Task<bool> AddCakeBase(int pid, int sid, int cbid)
        {
            try
            {
                PizzaOption pizzaOption = new PizzaOption()
                {
                    PizzaId = pid,
                    SizeId = sid,
                    CakeBaseId = cbid
                };
                _dbContext.PizzaOptions.Add(pizzaOption);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Order>> GetAllOrder(int currentPage, int item_per_page)
        {
            try{
            List<Order> orders = await _dbContext.Orders.Include(x => x.User).OrderByDescending(order => order.CreatedAt).Skip((currentPage - 1) * item_per_page)
                                          .Take(item_per_page).ToListAsync();
            return orders;
            }
            catch(Exception ex){
                return null;
            }
        }

        public async Task<int> FilterOrderCount(string OrderId, string CustomerName, string Email, string Status, string Total,
        string FromDate, string ToDate)
        {
            var query = _dbContext.Orders.Include(x => x.User).AsQueryable();

            if (!string.IsNullOrEmpty(OrderId))
            {
                query = query.Where(p => p.OrderId == int.Parse(OrderId));
            }

            if (!string.IsNullOrEmpty(CustomerName))
            {
                query = query.Where(p => p.User.UserName == CustomerName);
            }

            if (!string.IsNullOrEmpty(Total))
            {
                if (decimal.TryParse(Total, out decimal parsedPrice))
                {
                    query = query.Where(p => p.OrderTotal == parsedPrice);

                }
            }

            if (!string.IsNullOrEmpty(Email))
            {
                query = query.Where(p => p.User.Email == Email);

            }

            if (!string.IsNullOrEmpty(Status))
            {
                query = query.Where(p => p.State == Status);

            }

            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                if (DateTime.TryParse(FromDate, out DateTime parsedFromDate) && DateTime.TryParse(ToDate, out DateTime parsedToDate))
                {
                    if (parsedFromDate != default && parsedToDate != default)
                    {
                        query = query.Where(x => DateTime.Parse(x.CreatedAt.ToString("MM-dd-yyyy")) <= parsedToDate.Date && DateTime.Parse(x.CreatedAt.ToString("MM-dd-yyyy")) >= parsedToDate);
                    }
                }
            }
            var filteredOrders = await query.OrderByDescending(order => order.CreatedAt).ToListAsync();
            return filteredOrders.Count();
        }

        public async Task<List<Order>> FilterOrder(string OrderId, string CustomerName, string Email, string Status, string Total,
        string FromDate, string ToDate, int currentPage, int itemsPerPage)
        {
            var query = _dbContext.Orders.Include(x => x.User).AsQueryable();

            if (!string.IsNullOrEmpty(OrderId))
            {
                query = query.Where(p => p.OrderId == int.Parse(OrderId));
            }

            if (!string.IsNullOrEmpty(CustomerName))
            {
                query = query.Where(p => p.User.UserName == CustomerName);
            }

            if (!string.IsNullOrEmpty(Total))
            {
                if (decimal.TryParse(Total, out decimal parsedPrice))
                {
                    query = query.Where(p => p.OrderTotal == parsedPrice);

                }
            }

            if (!string.IsNullOrEmpty(Email))
            {
                query = query.Where(p => p.User.Email == Email);

            }

            if (!string.IsNullOrEmpty(Status))
            {
                query = query.Where(p => p.State == Status);

            }

            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                if (DateTime.TryParse(FromDate, out DateTime parsedFromDate) && DateTime.TryParse(ToDate, out DateTime parsedToDate))
                {
                    if (parsedFromDate != default && parsedToDate != default)
                    {
                        query = query.Where(x => DateTime.Parse(x.CreatedAt.ToString("MM-dd-yyyy")) <= parsedToDate.Date && DateTime.Parse(x.CreatedAt.ToString("MM-dd-yyyy")) >= parsedToDate);
                    }
                }
            }
            currentPage = currentPage <= 0 ? 1 : currentPage;
            query = query.Skip((currentPage - 1) * itemsPerPage)
                         .Take(itemsPerPage);
            var filteredOrders = await query.OrderByDescending(order => order.CreatedAt).ToListAsync();
            return filteredOrders;
        }

        public async Task<Order> GetOrderByOrderId(int orderId)
        {
            try
            {
                Order? order = await _dbContext.Orders.Include(x => x.User).FirstOrDefaultAsync(ord => ord.OrderId == orderId);
                if (order == null)
                {
                    return order;
                }
                else
                {
                    List<PizzaOrder> pizzaOrders = await _dbContext.PizzaOrders.Where(piodde => piodde.OrderId == orderId).ToListAsync();
                    order.PizzaOrders = pizzaOrders;
                    return order;
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<PizzaOrder>> GetListPizzaOrder(int orderId)
        {
            try
            {
                using (PRN221_PRJContext dbContext = new PRN221_PRJContext())
                {
                    List<PizzaOrder> pizza_order = dbContext.PizzaOrders
                        .Where(pizza_ord => pizza_ord.OrderId == orderId)
                        .ToList();

                    if (pizza_order == null)
                    {
                        return pizza_order;
                    }
                    else
                    {
                        var pizzaIds = pizza_order.Select(p => p.PizzaId).Distinct().ToList();
                        var sizeIds = pizza_order.Select(p => p.SizeId).Distinct().ToList();
                        var cakeBaseIds = pizza_order.Select(p => p.CakeBaseId).Distinct().ToList();
                        var orderIds = pizza_order.Select(p => p.OrderId).Distinct().ToList();

                        var pizzas = dbContext.Pizzas.Where(p => pizzaIds.Contains(p.Id)).ToList();
                        var sizes = dbContext.Sizes.Where(s => sizeIds.Contains(s.Id)).ToList();
                        var cakeBases = dbContext.CakeBases.Where(cb => cakeBaseIds.Contains(cb.Id)).ToList();
                        var orders = dbContext.Orders.Where(ord => orderIds.Contains(ord.OrderId)).ToList();

                        foreach (var pizza_ord in pizza_order)
                        {
                            pizza_ord.Pizza = pizzas.FirstOrDefault(p => p.Id == pizza_ord.PizzaId);
                            pizza_ord.Size = sizes.FirstOrDefault(s => s.Id == pizza_ord.SizeId);
                            pizza_ord.CakeBase = cakeBases.FirstOrDefault(cb => cb.Id == pizza_ord.CakeBaseId);
                            pizza_ord.Order = orders.FirstOrDefault(ord => ord.OrderId == pizza_ord.OrderId);
                        }

                        return pizza_order;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return null;
            }
        }

    }
}
