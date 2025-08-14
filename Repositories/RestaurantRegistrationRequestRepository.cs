using FoodDeliveryBackend.Data;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodDeliveryBackend.Repositories
{
    public class RestaurantRegistrationRequestRepository : Repository<RestaurantRegistrationRequest>, IRestaurantRegistrationRequestRepository
    {
        public RestaurantRegistrationRequestRepository(AppDbContext context) : base(context)
        {
        }
    }
}
