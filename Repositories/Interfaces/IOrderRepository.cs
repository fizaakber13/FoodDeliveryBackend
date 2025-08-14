using FoodDeliveryBackend.Models;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using System.Threading.Tasks;

namespace FoodDeliveryBackend.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
    }
}