using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SocialPay.Core.Services
{
    public abstract class BaseService<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        protected readonly DbSet<TEntity> Entities;

        protected BaseService(DbContext context)
        {
            _context = context;
            Entities = context.Set<TEntity>();
        }


        public virtual Task<List<TEntity>> GetAsync()
        {
            return Entities.ToListAsync();
        }

        public virtual Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Entities.Where(predicate).ToListAsync();
        }

        public virtual Task<long> CountAsync()
        {
            return Entities.LongCountAsync();
        }

        public virtual Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Entities.LongCountAsync(predicate);
        }

        public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Entities.AnyAsync(predicate);
        }

        public async Task SaveAsync(TEntity entity)
        {
            Entities.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
        }
    }

}
