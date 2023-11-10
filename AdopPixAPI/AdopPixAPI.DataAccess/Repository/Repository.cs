using AdopPixAPI.DataAccess.ApplicationDbContext;
using AdopPixAPI.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AdopPixAPI.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly SqlServerDbContext context;
        protected DbSet<T> dbSet;

        public Repository(SqlServerDbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public virtual IQueryable<T> Find(Expression<Func<T, bool>> expression)
        {
            try
            {
                return context.Set<T>().Where(expression);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            } 
        }

        public virtual async Task<T> FindById<N>(N id)
        {
            try
            {
                return await dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        public virtual async Task Add(T entity)
        {
            try
            {
                await dbSet.AddAsync(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public virtual async Task AddRange(IEnumerable<T> entities)
        {
            try
            {
                await dbSet.AddRangeAsync(entities);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public virtual void Update(T entity)
        {
            try
            {
                dbSet.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public virtual void Remove(T entity)
        {
            try
            {
                dbSet.Remove(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            try
            {
                dbSet.RemoveRange(entities);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
