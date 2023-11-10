using System.Linq.Expressions;

namespace AdopPixAPI.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Find(Expression<Func<T, bool>> expression);
        Task<T> FindById<N>(N id);
        Task Add(T entity);
        Task AddRange(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
