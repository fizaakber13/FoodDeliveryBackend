using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Services.Interfaces;
using FoodDeliveryBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDeliveryBackend.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CartItemResponse?> GetCartItemByIdAsync(int id)
        {
            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(id);
            if (cartItem == null)
            {
                return null;
            }
            return new CartItemResponse(cartItem.Id, cartItem.UserId, cartItem.RestaurantId, cartItem.MenuItemId, cartItem.Quantity);
        }

        public async Task<IEnumerable<CartItemResponse>> GetAllCartItemsAsync()
        {
            var cartItems = await _unitOfWork.CartItems.GetAllAsync();
            return cartItems.Select(cartItem => new CartItemResponse(cartItem.Id, cartItem.UserId, cartItem.RestaurantId, cartItem.MenuItemId, cartItem.Quantity));
        }

        public async Task<CartItemResponse> CreateCartItemAsync(CreateCartItemRequest cartItemDto)
        {
            var cartItem = new CartItem
            {
                UserId = cartItemDto.UserId,
                RestaurantId = cartItemDto.RestaurantId,
                MenuItemId = cartItemDto.MenuItemId,
                Quantity = cartItemDto.Quantity
            };
            await _unitOfWork.CartItems.AddAsync(cartItem);
            await _unitOfWork.SaveChangesAsync();
            return new CartItemResponse(cartItem.Id, cartItem.UserId, cartItem.RestaurantId, cartItem.MenuItemId, cartItem.Quantity);
        }

        public async Task DeleteCartItemAsync(int id)
        {
            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(id);
            if (cartItem == null) return;

            _unitOfWork.CartItems.Remove(cartItem);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<CartItemResponse>> GetCartItemsByUserIdAsync(int userId)
        {
            var cartItems = await _unitOfWork.CartItems.FindAsync(ci => ci.UserId == userId);
            return cartItems.Select(cartItem => new CartItemResponse(cartItem.Id, cartItem.UserId, cartItem.RestaurantId, cartItem.MenuItemId, cartItem.Quantity));
        }

        public async Task<List<CartSummaryResponse>> GetCartSummaryByUserAsync(int userId)
        {
            var cartItems = await _unitOfWork.CartItems.GetAsync(
                ci => ci.UserId == userId,
                ci => ci.Restaurant,
                ci => ci.MenuItem
            );

            var groupedCart = cartItems.Where(ci => ci.Restaurant != null)
                .GroupBy(ci => new { RestaurantId = ci.Restaurant!.Id, RestaurantName = ci.Restaurant!.Name })
                .Select(group =>
                {
                    if (group.Key == null) return null;
                    return new CartSummaryResponse(
                        group.Key.RestaurantId,
                        group.Key.RestaurantName,
                        group.Sum(ci => (ci.MenuItem != null ? ci.MenuItem.Price : 0) * ci.Quantity),
                        group.Select(ci =>
                        {
                            if (ci.MenuItem == null) return null;
                            return new CartSummaryItemResponse(
                                ci.Id,
                                ci.MenuItemId,
                                ci.MenuItem.Name,
                                ci.Quantity,
                                ci.MenuItem.Price,
                                ci.MenuItem.Price * ci.Quantity
                            );
                        }).Where(item => item != null).ToList()! // Use ! to assert non-null after Where(item => item != null)
                    );
                }).Where(dto => dto != null).ToList()!;

            return groupedCart;
        }

        public async Task<decimal> GetCartTotalAsync(int userId, int restaurantId)
        {
            var total = (await _unitOfWork.CartItems.GetAsync(
                c => c.UserId == userId && c.RestaurantId == restaurantId,
                includeProperties: c => c.MenuItem
            ))
            .Where(c => c.MenuItem != null)
            .Sum(c => (c.MenuItem!.Price) * c.Quantity);

            return total;
        }

        public async Task<CouponApplicationResultResponse> GetCartTotalWithCouponAsync(int userId, int restaurantId, string couponCode)
        {
            var cartItems = await _unitOfWork.CartItems.GetAsync(
                ci => ci.UserId == userId && ci.RestaurantId == restaurantId,
                ci => ci.MenuItem
            );

            if (!cartItems.Any())
                return new CouponApplicationResultResponse(false, "Cart is empty.", null, null, null);

            var total = cartItems.Where(ci => ci.MenuItem != null).Sum(ci => (ci.MenuItem!.Price) * ci.Quantity);

            var coupon = await _unitOfWork.RestaurantCoupons.FirstOrDefaultAsync(
                rc => rc.RestaurantId == restaurantId && rc.Coupon != null && rc.Coupon.Code == couponCode && rc.Coupon.IsActive,
                rc => rc.Coupon
            );

            if (coupon?.Coupon == null)
                return new CouponApplicationResultResponse(false, "Invalid or inactive coupon.", null, null, null);

            var actualCoupon = coupon.Coupon;

            if (total < actualCoupon.MinOrderAmount)
                return new CouponApplicationResultResponse(
                    false,
                    $"Minimum order ₹{actualCoupon.MinOrderAmount} required to apply this coupon.",
                    null, null, null
                );

            decimal discount = 0;

            if (actualCoupon.DiscountType == Models.DiscountType.Flat)
                discount = actualCoupon.DiscountAmount;
            else if (actualCoupon.DiscountType == Models.DiscountType.Percentage)
                discount = total * (actualCoupon.DiscountAmount / 100);

            return new CouponApplicationResultResponse(
                true,
                "Coupon applied successfully.",
                actualCoupon.Code,
                discount,
                actualCoupon.DiscountType
            );
        }

        public async Task UpdateCartItemQuantityAsync(int id, int newQuantity)
        {
            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(id);
            if (cartItem == null) return;

            if (newQuantity <= 0)
            {
                _unitOfWork.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = newQuantity;
                _unitOfWork.CartItems.Update(cartItem);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(int id, CartItemResponse cartItemDto)
        {
            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(id);
            if (cartItem == null) return;

            cartItem.UserId = cartItemDto.UserId;
            cartItem.RestaurantId = cartItemDto.RestaurantId;
            cartItem.MenuItemId = cartItemDto.MenuItemId;
            cartItem.Quantity = cartItemDto.Quantity;

            _unitOfWork.CartItems.Update(cartItem);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<CartItemResponse> AddOrUpdateCartItemAsync(int userId, int menuItemId, int quantity = 1)
        {
            var existingCartItem = await _unitOfWork.CartItems.FirstOrDefaultAsync(
                ci => ci.UserId == userId && ci.MenuItemId == menuItemId
            );

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
                _unitOfWork.CartItems.Update(existingCartItem);
            }
            else
            {
                // Need to get RestaurantId from MenuItem
                var menuItem = await _unitOfWork.MenuItems.GetByIdAsync(menuItemId);
                if (menuItem == null)
                {
                    throw new System.Exception("Menu item not found.");
                }

                var newCartItem = new CartItem
                {
                    UserId = userId,
                    RestaurantId = menuItem.RestaurantId,
                    MenuItemId = menuItemId,
                    Quantity = quantity
                };
                await _unitOfWork.CartItems.AddAsync(newCartItem);
                existingCartItem = newCartItem; // Assign new item to existingCartItem for response
            }

            await _unitOfWork.SaveChangesAsync();
            return new CartItemResponse(existingCartItem.Id, existingCartItem.UserId, existingCartItem.RestaurantId, existingCartItem.MenuItemId, existingCartItem.Quantity);
        }

        public async Task<CartItemResponse?> RemoveOrDecrementCartItemAsync(int userId, int menuItemId, int quantity = 1)
        {
            var existingCartItem = await _unitOfWork.CartItems.FirstOrDefaultAsync(
                ci => ci.UserId == userId && ci.MenuItemId == menuItemId
            );

            if (existingCartItem == null) return null; // Nothing to remove

            existingCartItem.Quantity -= quantity;

            if (existingCartItem.Quantity <= 0)
            {
                _unitOfWork.CartItems.Remove(existingCartItem);
                await _unitOfWork.SaveChangesAsync();
                return null; // Item removed
            }
            else
            {
                _unitOfWork.CartItems.Update(existingCartItem);
                await _unitOfWork.SaveChangesAsync();
                return new CartItemResponse(existingCartItem.Id, existingCartItem.UserId, existingCartItem.RestaurantId, existingCartItem.MenuItemId, existingCartItem.Quantity);
            }
        }

        public async Task ClearCartAsync(int userId, int restaurantId)
        {
            var cartItems = await _unitOfWork.CartItems.GetAsync(ci => ci.UserId == userId && ci.RestaurantId == restaurantId);
            foreach (var cartItem in cartItems)
            {
                _unitOfWork.CartItems.Remove(cartItem);
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
