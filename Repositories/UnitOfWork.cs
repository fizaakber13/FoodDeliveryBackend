using FoodDeliveryBackend.Data;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace FoodDeliveryBackend.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IAddressRepository Addresses { get; private set; }
        public ICartItemRepository CartItems { get; private set; }
        public ICouponRepository Coupons { get; private set; }
        public IMenuItemRepository MenuItems { get; private set; }
        public IOrderRepository Orders { get; private set; }
        public IOrderItemRepository OrderItems { get; private set; }
        public IRestaurantRepository Restaurants { get; private set; }
        public IRestaurantCouponRepository RestaurantCoupons { get; private set; }
        public IRestaurantRegistrationRequestRepository RestaurantRegistrationRequests { get; private set; }
        public IUserRepository Users { get; private set; }
        public IOtpEntryRepository OtpEntries { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Addresses = new AddressRepository(_context);
            CartItems = new CartItemRepository(_context);
            Coupons = new CouponRepository(_context);
            MenuItems = new MenuItemRepository(_context);
            Orders = new OrderRepository(_context);
            OrderItems = new OrderItemRepository(_context);
            Restaurants = new RestaurantRepository(_context);
            RestaurantCoupons = new RestaurantCouponRepository(_context);
            RestaurantRegistrationRequests = new RestaurantRegistrationRequestRepository(_context);
            Users = new UserRepository(_context);
            OtpEntries = new OtpEntryRepository(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}