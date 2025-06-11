using Microsoft.EntityFrameworkCore;
using FoodDeliveryBackend.Models;

namespace FoodDeliveryBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<RestaurantCoupon> RestaurantCoupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User - Address
            modelBuilder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade); 

            // Restaurant - MenuItem
            modelBuilder.Entity<MenuItem>()
                .HasOne(m => m.Restaurant)
                .WithMany(r => r.MenuItems)
                .HasForeignKey(m => m.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade); 

            // User - CartItem
            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            // Restaurant - CartItem
            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Restaurant)
                .WithMany()
                .HasForeignKey(c => c.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            // MenuItem - CartItem
            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.MenuItem)
                .WithMany()
                .HasForeignKey(c => c.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            // Restaurant - Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Restaurant)
                .WithMany(r => r.Orders)
                .HasForeignKey(o => o.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order - OrderItem
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // MenuItem - OrderItem
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.MenuItem)
                .WithMany()
                .HasForeignKey(oi => oi.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // RestaurantCoupon
            modelBuilder.Entity<RestaurantCoupon>()
                .HasOne(rc => rc.Restaurant)
                .WithMany(r => r.RestaurantCoupons)
                .HasForeignKey(rc => rc.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RestaurantCoupon>()
                .HasOne(rc => rc.Coupon)
                .WithMany(c => c.RestaurantCoupons)
                .HasForeignKey(rc => rc.CouponId)
                .OnDelete(DeleteBehavior.Restrict);

            

            modelBuilder.Entity<Coupon>()
                .Property(c => c.DiscountAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Coupon>()
                .Property(c => c.MinOrderAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

           

            base.OnModelCreating(modelBuilder);
        }
    }
}
