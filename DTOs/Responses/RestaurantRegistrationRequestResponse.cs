using System;

namespace FoodDeliveryBackend.DTOs.Responses
{
    public record RestaurantRegistrationRequestResponse(int Id, string RestaurantName, string Email, string Contact, string Location, DateTime RequestedAt);
}