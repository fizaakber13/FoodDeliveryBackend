using FoodDeliveryBackend.Models;
using System;
using System.Threading.Tasks;

namespace FoodDeliveryBackend.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAddressRepository Addresses { get; }
        ICartItemRepository CartItems { get; }
        ICouponRepository Coupons { get; }
        IMenuItemRepository MenuItems { get; }
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }
        IRestaurantRepository Restaurants { get; }
        IRestaurantCouponRepository RestaurantCoupons { get; }
        IRestaurantRegistrationRequestRepository RestaurantRegistrationRequests { get; }
        IUserRepository Users { get; }
        IOtpEntryRepository OtpEntries { get; }

        Task<int> SaveChangesAsync();
    }
}