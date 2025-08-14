using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using FoodDeliveryBackend.Models;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodDeliveryBackend.Services.Interfaces;
using FoodDeliveryBackend.Repositories.Interfaces;

namespace FoodDeliveryBackend.Services.Implementations
{
    public class RestaurantRegistrationRequestService : IRestaurantRegistrationRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RestaurantRegistrationRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RestaurantRegistrationRequestResponse?> GetRestaurantRegistrationRequestByIdAsync(int id)
        {
            var request = await _unitOfWork.RestaurantRegistrationRequests.GetByIdAsync(id);
            if (request == null) return null;
            return new RestaurantRegistrationRequestResponse(request.Id, request.RestaurantName, request.Email, request.Contact, request.Location, request.RequestedAt);
        }

        public async Task<IEnumerable<RestaurantRegistrationRequestResponse>> GetAllRestaurantRegistrationRequestsAsync()
        {
            var requests = await _unitOfWork.RestaurantRegistrationRequests.GetAllAsync();
            return requests.Select(r => new RestaurantRegistrationRequestResponse(r.Id, r.RestaurantName, r.Email, r.Contact, r.Location, r.RequestedAt));
        }

        public async Task<RestaurantRegistrationRequestResponse> CreateRestaurantRegistrationRequestAsync(CreateRestaurantRegistrationRequest requestDto)
        {
            var request = new RestaurantRegistrationRequest
            {
                RestaurantName = requestDto.RestaurantName,
                Email = requestDto.Email,
                Contact = requestDto.Contact,
                Location = requestDto.Location,
                RequestedAt = DateTime.UtcNow
            };
            await _unitOfWork.RestaurantRegistrationRequests.AddAsync(request);
            await _unitOfWork.SaveChangesAsync();
            return new RestaurantRegistrationRequestResponse(request.Id, request.RestaurantName, request.Email, request.Contact, request.Location, request.RequestedAt);
        }

        public async Task DeleteRestaurantRegistrationRequestAsync(int id)
        {
            var request = await _unitOfWork.RestaurantRegistrationRequests.GetByIdAsync(id);
            if (request == null) return;

            _unitOfWork.RestaurantRegistrationRequests.Remove(request);
            await _unitOfWork.SaveChangesAsync();
        }

        
    }
}