using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CheckInGWDN.Data;
using CheckInGWDN.Services.IRepository;
using Microsoft.EntityFrameworkCore;

namespace CheckInGWDN.Services
{
    public class Repository<T> : IRepositoryNormal<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            this._db = db;
            this.dbSet = db.Set<T>();
        }

        public T Get(int id)
        {
            return dbSet.Find(id);
        }

        public T Get(string id)
        {
            return dbSet.Find(id);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null,
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
                return orderBy(query).ToList();
            }

            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null)
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

            return query.FirstOrDefault();
        }

        public void AddList(List<T> entities)
        {
            dbSet.AddRange(entities);
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public void Remove(int id)
        {
            T entity = dbSet.Find(id);
            Remove(entity);
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}