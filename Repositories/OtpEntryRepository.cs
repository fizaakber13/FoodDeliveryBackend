using FoodDeliveryBackend.Data;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Repositories.Interfaces;

namespace FoodDeliveryBackend.Repositories
{
    public class OtpEntryRepository : Repository<OtpEntry>, IOtpEntryRepository
    {
        public OtpEntryRepository(AppDbContext context) : base(context)
        {
        }
    }
}
