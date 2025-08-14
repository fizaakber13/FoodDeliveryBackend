using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodDeliveryBackend.Repositories.Interfaces;

namespace FoodDeliveryBackend.Services.Implementations
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddressService(IUnitOfWork unitOfWork, AutoMapper.IMapper @object)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AddressResponse?> GetAddressByIdAsync(int id)
        {
            var address = await _unitOfWork.Addresses.GetByIdAsync(id);
            if (address == null) return null;
            return new AddressResponse(address.Id, address.Line, address.IsDefault, address.Label, address.UserId);
        }

        public async Task<IEnumerable<AddressResponse>> GetAllAddressesAsync()
        {
            var addresses = await _unitOfWork.Addresses.GetAllAsync();
            return addresses.Select(a => new AddressResponse(a.Id, a.Line, a.IsDefault, a.Label, a.UserId));
        }

        public async Task<AddressResponse> CreateAddressAsync(CreateAddressRequest addressDto)
        {
            var address = new Address
            {
                Line = addressDto.Line,
                IsDefault = addressDto.IsDefault,
                Label = addressDto.Label,
                UserId = addressDto.UserId
            };
            await _unitOfWork.Addresses.AddAsync(address);
            await _unitOfWork.SaveChangesAsync();
            return new AddressResponse(address.Id, address.Line, address.IsDefault, address.Label, address.UserId);
        }

        public async Task UpdateAddressAsync(int id, AddressResponse addressDto)
        {
            var address = await _unitOfWork.Addresses.GetByIdAsync(id);
            if (address == null) return;

            address.Line = addressDto.Line;
            address.IsDefault = addressDto.IsDefault;
            address.Label = addressDto.Label;
            address.UserId = addressDto.UserId;

            _unitOfWork.Addresses.Update(address);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAddressAsync(int id)
        {
            var address = await _unitOfWork.Addresses.GetByIdAsync(id);
            if (address == null) return;

            _unitOfWork.Addresses.Remove(address);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<AddressResponse>> GetAddressesByUserIdAsync(int userId)
        {
            var addresses = await _unitOfWork.Addresses.FindAsync(a => a.UserId == userId);
            return addresses.Select(a => new AddressResponse(a.Id, a.Line, a.IsDefault, a.Label, a.UserId));
        }
    }
}