using FoodDeliveryBackend.Data;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodDeliveryBackend.Repositories
{
    public class RestaurantCouponRepository : Repository<RestaurantCoupon>, IRestaurantCouponRepository
    {
        public RestaurantCouponRepository(AppDbContext context) : base(context)
        {
        }
    }
}
