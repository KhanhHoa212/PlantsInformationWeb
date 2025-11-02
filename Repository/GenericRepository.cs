using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlantsInformationWeb.Models;

namespace PlantsInformationWeb.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly PlantsInformationContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(PlantsInformationContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(object id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<T>> GetAllByIdAsync(IEnumerable<int> ids)
        {
            var keyProperty = typeof(T).GetProperties().FirstOrDefault(p => p.Name.EndsWith("Id"));
            if (keyProperty == null)
                throw new InvalidOperationException("Không tìm thấy khóa chính có tên kết thúc bằng 'Id'.");

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, keyProperty.Name);
            var containsMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(int));
            var idsExpression = Expression.Constant(ids);
            var body = Expression.Call(containsMethod, idsExpression, property);
            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);

            return await _context.Set<T>().Where(lambda).ToListAsync();
        }

        public IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }


    }
}