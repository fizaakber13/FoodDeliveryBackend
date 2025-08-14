using FoodDeliveryBackend.Data;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodDeliveryBackend.Repositories
{
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {
        public CouponRepository(AppDbContext context) : base(context)
        {
        }
    }
}
