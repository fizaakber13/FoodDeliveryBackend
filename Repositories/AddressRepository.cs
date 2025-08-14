using FoodDeliveryBackend.Data;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodDeliveryBackend.Repositories
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        public AddressRepository(AppDbContext context) : base(context)
        {
        }
    }
}
