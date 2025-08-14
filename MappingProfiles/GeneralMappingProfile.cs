using AutoMapper;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using FoodDeliveryBackend.DTOs.Requests.Auth;
using FoodDeliveryBackend.DTOs.Responses.Auth;

namespace FoodDeliveryBackend.MappingProfiles
{
    public class GeneralMappingProfile : Profile
    {
        public GeneralMappingProfile()
        {
            // Requests to Models
            CreateMap<CreateUserRequest, User>();
            CreateMap<CreateAddressRequest, Address>();
            CreateMap<CreateCartItemRequest, CartItem>();
            CreateMap<CreateCouponRequest, Coupon>();
            CreateMap<CreateMenuItemRequest, MenuItem>();
            CreateMap<CreateOrderRequest, Order>();
            CreateMap<CreateOrderItemRequest, OrderItem>();
            CreateMap<CreateRestaurantCouponRequest, RestaurantCoupon>();
            CreateMap<CreateRestaurantRequest, Restaurant>();
            CreateMap<CreateRestaurantRegistrationRequest, RestaurantRegistrationRequest>();
            CreateMap<RegisterRequest, User>(); // Assuming RegisterRequest maps to User model

            // Models to Responses
            CreateMap<User, UserResponse>();
            CreateMap<Address, AddressResponse>();
            CreateMap<CartItem, CartItemResponse>();
            CreateMap<Coupon, CouponResponse>();
            CreateMap<MenuItem, MenuItemResponse>();
            CreateMap<Order, OrderResponse>();
            CreateMap<OrderItem, OrderItemResponse>();
            CreateMap<RestaurantCoupon, RestaurantCouponResponse>();
            CreateMap<Restaurant, RestaurantResponse>();
            CreateMap<RestaurantRegistrationRequest, RestaurantRegistrationRequestResponse>();

            // DTO to DTO mappings (for nested DTOs or specific response structures)
            CreateMap<CartSummaryResponse, CartSummaryItemResponse>(); // Assuming CartSummaryItemDto is an internal DTO
            CreateMap<CartTotalResponse, CartTotalResponse>(); // Assuming CartTotalDto is an internal DTO
            CreateMap<CouponApplicationResultResponse, CouponApplicationResultResponse>(); // Assuming CouponApplicationResultDto is an internal DTO
        }
    }
}