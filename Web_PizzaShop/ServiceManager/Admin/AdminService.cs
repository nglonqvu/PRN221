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

            var query = _dbContext.Pizzas.Skip((currentPage - 1) * item_per_page)
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
            var filteredPizzas = await query.ToListAsync();
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

            List<Pizza> filteredPizzas = await query.ToListAsync();
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
            List<Category> categories = await _dbContext.Categories.ToListAsync();
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
            List<PizzaOption> pizzaOptions = await _dbContext.PizzaOptions.Where(pro => pro.PizzaId == pizzaid).ToListAsync();
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
                PizzaOption pizzaOption = new PizzaOption(){
                    CakeBaseId = cakeBasis.Id,
                    PizzaId = pizzaId,
                    SizeId = sizeId
                };
                await _dbContext.PizzaOptions.AddAsync(pizzaOption);
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

        public async Task<bool> AddCakeBase(int pid, int sid, int cbid){
            try{
            PizzaOption pizzaOption = new PizzaOption(){
                PizzaId = pid,
                SizeId = sid,
                CakeBaseId = cbid
            };
            _dbContext.PizzaOptions.Add(pizzaOption);
            await _dbContext.SaveChangesAsync();
            return true;
            }
            catch{
                return false;
            }
        }
    }
}
