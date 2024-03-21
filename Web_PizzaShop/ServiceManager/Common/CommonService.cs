using Microsoft.EntityFrameworkCore;
using Web_PizzaShop.Interface.Admin;
using Web_PizzaShop.Interface.Common;
using Web_PizzaShop.Models;

namespace Web_PizzaShop.ServiceManager.Common
{
    public class CommonService : ICommonService
    {
        private readonly PRN221_PRJContext _dbContext;

        public CommonService(PRN221_PRJContext dbContext)
        {
            _dbContext = dbContext;
        }

        
    }
}
