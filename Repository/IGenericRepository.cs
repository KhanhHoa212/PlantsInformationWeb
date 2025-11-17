using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PlantsInformationWeb.Models;

namespace PlantsInformationWeb.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(object id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(object id);
        Task<List<T>> GetAllByIdAsync(IEnumerable<int> ids);
        IQueryable<T> GetQueryable();
        IQueryable<T> GetQueryableWithIncludes(params Expression<Func<T, object>>[] includes);
        Task<int> GetCountAsync(Expression<Func<T, bool>>? predicate = null);

    }
}