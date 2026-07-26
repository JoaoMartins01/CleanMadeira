using CleanMadeira.Application.Common.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Common.Interfaces
{
    public interface IPropertyRepository
    : IRepository<Property>
    {
        Task<IEnumerable<Property>> GetByApplicationUserAsync(Guid guid);
        Task<IEnumerable<Property>> GetByUserIdAsync(Guid guid);
        Task<Property?> GetByIdWithOwnerAsync(Guid id);
        Task<Property?> GetByIdAndOwnerAsync(Guid id, Guid ownerId);
        Task<List<Property>> GetInactiveAsync(Guid guid);
        Task<int> CountInactiveAsync(Guid id);
    }
}
