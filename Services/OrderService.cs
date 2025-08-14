using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Services.Interfaces;
using FoodDeliveryBackend.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FoodDeliveryBackend.Pagination;

namespace FoodDeliveryBackend.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartItemService _cartItemService;

        public OrderService(IUnitOfWork unitOfWork, ICartItemService cartItemService)
        {
            _unitOfWork = unitOfWork;
            _cartItemService = cartItemService;
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.Id == id, o => o.OrderItems);
            if (order == null) return null;

            // Explicitly load MenuItem for each OrderItem
            foreach (var orderItem in order.OrderItems)
            {
                orderItem.MenuItem = await _unitOfWork.MenuItems.GetByIdAsync(orderItem.MenuItemId);
            }

            return new OrderResponse(
                order.Id,
                order.UserId,
                order.RestaurantId,
                order.OrderDate,
                order.TotalAmount,
                order.Status,
                order.Address,
                order.PaymentMethod,
                order.OrderItems.Select(oi => new OrderItemResponse(oi.Id, oi.MenuItemId, oi.Quantity, oi.Price, oi.MenuItem!.Name)).ToList()
            );
        }

        public async Task<PagedList<OrderResponse>> GetAllOrdersAsync(PaginationParams paginationParams)
        {
            var source = _unitOfWork.Orders.GetAllAsQueryable();

            var pagedList = await PagedList<Order>.CreateAsync(source, paginationParams.PageNumber, paginationParams.PageSize);

            var orderResponses = new List<OrderResponse>();
            foreach (var order in pagedList)
            {
                orderResponses.Add(new OrderResponse(
                    order.Id,
                    order.UserId,
                    order.RestaurantId,
                    order.OrderDate,
                    order.TotalAmount,
                    order.Status,
                    order.Address,
                    order.PaymentMethod,
                    order.OrderItems.Select(oi => new OrderItemResponse(oi.Id, oi.MenuItemId, oi.Quantity, oi.Price, oi.MenuItem?.Name ?? "Unknown Item")).ToList()
                ));
            }

            return new PagedList<OrderResponse>(orderResponses, pagedList.TotalCount, pagedList.CurrentPage, pagedList.PageSize);
        }

        public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest orderDto)
        {
            var address = await _unitOfWork.Addresses.GetByIdAsync(orderDto.AddressId);
            if (address == null)
            {
                throw new Exception("Address not found.");
            }

            var order = new Order
            {
                UserId = orderDto.UserId,
                RestaurantId = orderDto.RestaurantId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                Address = address.Line,
                PaymentMethod = orderDto.PaymentMethod
            };

            order.OrderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var itemDto in orderDto.OrderItems)
            {
                var menuItem = await _unitOfWork.MenuItems.GetByIdAsync(itemDto.MenuItemId);
                if (menuItem == null)
                {
                    throw new Exception($"Menu item with ID {itemDto.MenuItemId} not found.");
                }

                var orderItem = new OrderItem
                {
                    MenuItemId = itemDto.MenuItemId,
                    Quantity = itemDto.Quantity,
                    Price = (decimal)menuItem.Price
                };
                order.OrderItems.Add(orderItem);
                totalAmount += orderItem.Quantity * orderItem.Price;
            }

            // if (!string.IsNullOrEmpty(orderDto.CouponCode))
            // {
            //     var coupon = await _unitOfWork.RestaurantCoupons.FirstOrDefaultAsync(
            //         rc => rc.RestaurantId == orderDto.RestaurantId && rc.Coupon != null && rc.Coupon.Code == orderDto.CouponCode && rc.Coupon.IsActive,
            //         rc => rc.Coupon
            //     );

            //     if (coupon != null && coupon.Coupon != null)
            //     {
            //         var actualCoupon = coupon.Coupon;
            //         if (totalAmount >= actualCoupon.MinOrderAmount)
            //         {
            //             if (actualCoupon.DiscountType == DiscountType.Flat)
            //             {
            //                 totalAmount -= actualCoupon.DiscountAmount;
            //             }
            //             else
            //             {
            //                 totalAmount -= totalAmount * (actualCoupon.DiscountAmount / 100);
            //             }
            //         }
            //     }
            // }

            order.TotalAmount = totalAmount;

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            // Explicitly load MenuItem for each OrderItem before mapping
            foreach (var orderItem in order.OrderItems)
            {
                orderItem.MenuItem = await _unitOfWork.MenuItems.GetByIdAsync(orderItem.MenuItemId);
            }

            await _cartItemService.ClearCartAsync(orderDto.UserId, orderDto.RestaurantId);

            return new OrderResponse(
                order.Id,
                order.UserId,
                order.RestaurantId,
                order.OrderDate,
                order.TotalAmount,
                order.Status,
                order.Address,
                order.PaymentMethod,
                order.OrderItems.Select(oi => new OrderItemResponse(oi.Id, oi.MenuItemId, oi.Quantity, oi.Price, oi.MenuItem!.Name)).ToList()
            );
        }

        public async Task UpdateOrderAsync(int id, OrderResponse orderDto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null) return;

            order.UserId = orderDto.UserId;
            order.RestaurantId = orderDto.RestaurantId;
            order.OrderDate = orderDto.OrderDate;
            order.TotalAmount = orderDto.TotalAmount;
            order.Status = orderDto.Status;
            order.Address = orderDto.Address;
            order.PaymentMethod = orderDto.PaymentMethod;

            // For simplicity, assuming order items are not updated via this method
            // If they are, more complex logic would be needed to handle additions/removals/updates

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null) return;

            _unitOfWork.Orders.Remove(order);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderResponse>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _unitOfWork.Orders.GetAsync(o => o.UserId == userId, o => o.Restaurant, o => o.OrderItems);

            foreach (var order in orders)
            {
                foreach (var orderItem in order.OrderItems)
                {
                    orderItem.MenuItem = await _unitOfWork.MenuItems.GetByIdAsync(orderItem.MenuItemId);
                }
            }

            return orders.Select(order => new OrderResponse(
                order.Id,
                order.UserId,
                order.RestaurantId,
                order.OrderDate,
                order.TotalAmount,
                order.Status,
                order.Address,
                order.PaymentMethod,
                order.OrderItems!.Select(oi => new OrderItemResponse(oi.Id, oi.MenuItemId, oi.Quantity, oi.Price, oi.MenuItem!.Name)).ToList()
            ));
        }

        public async Task<IEnumerable<OrderResponse>> GetOrdersByRestaurantIdAsync(int restaurantId)
        {
            var orders = await _unitOfWork.Orders.GetAsync(o => o.RestaurantId == restaurantId, o => o.User, o => o.OrderItems);

            foreach (var order in orders)
            {
                foreach (var orderItem in order.OrderItems)
                {
                    orderItem.MenuItem = await _unitOfWork.MenuItems.GetByIdAsync(orderItem.MenuItemId);
                }
            }

            return orders.Select(order => new OrderResponse(
                order.Id,
                order.UserId,
                order.RestaurantId,
                order.OrderDate,
                order.TotalAmount,
                order.Status,
                order.Address,
                order.PaymentMethod,
                order.OrderItems!.Select(oi => new OrderItemResponse(oi.Id, oi.MenuItemId, oi.Quantity, oi.Price, oi.MenuItem!.Name)).ToList()
            ));
        }

        public async Task UpdateOrderStatusAsync(int id, string status)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null) return;

            order.Status = status;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderResponse>> SearchOrdersByRestaurantAsync(int userId, string restaurantName)
        {
            var orders = await _unitOfWork.Orders.GetAsync(
                o => o.UserId == userId && (string.IsNullOrEmpty(restaurantName) || o.Restaurant!.Name.Contains(restaurantName)),
                o => o.Restaurant, o => o.OrderItems
            );

            foreach (var order in orders)
            {
                foreach (var orderItem in order.OrderItems)
                {
                    orderItem.MenuItem = await _unitOfWork.MenuItems.GetByIdAsync(orderItem.MenuItemId);
                }
            }

            return orders.Select(order => new OrderResponse(
                order.Id,
                order.UserId,
                order.RestaurantId,
                order.OrderDate,
                order.TotalAmount,
                order.Status,
                order.Address,
                order.PaymentMethod,
                order.OrderItems!.Select(oi => new OrderItemResponse(oi.Id, oi.MenuItemId, oi.Quantity, oi.Price, oi.MenuItem!.Name)).ToList()
            ));
        }
    }
}