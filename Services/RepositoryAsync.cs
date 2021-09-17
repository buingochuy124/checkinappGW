using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CheckInGWDN.Data;
using CheckInGWDN.Services.IRepository;
using Microsoft.EntityFrameworkCore;

namespace CheckInGWDN.Services
{
    public class RepositoryAsync<T> : IRepositoryAsync<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public RepositoryAsync(ApplicationDbContext db)
        {
            this._db = db;
            this.dbSet = db.Set<T>();
        }

        public async Task<T> GetAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<T> GetAsync(string id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] {','},
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter = null,
            string includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] {','},
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task AddRangeAsync(List<T> entities)
        {
            await dbSet.AddRangeAsync(entities);
        }

        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task RemoveAsync(int id)
        {
            T entity = await dbSet.FindAsync(id);
            dbSet.Remove(entity);
        }

        public async Task RemoveRangeAsync(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
            await _db.DisposeAsync();
        }
    }
}