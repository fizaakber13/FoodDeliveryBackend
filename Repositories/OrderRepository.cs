using FoodDeliveryBackend.Data;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryBackend.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        public override IQueryable<Order> GetAllAsQueryable()
        {
            return _dbSet.Include(o => o.OrderItems).ThenInclude(oi => oi.MenuItem);
        }
    }
}
