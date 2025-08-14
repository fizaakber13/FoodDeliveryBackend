using FoodDeliveryBackend.Models;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace FoodDeliveryBackend.Repositories.Interfaces
{
    public interface IMenuItemRepository : IRepository<MenuItem>
    {
        IQueryable<MenuItem> GetAllAsQueryable();
    }
}