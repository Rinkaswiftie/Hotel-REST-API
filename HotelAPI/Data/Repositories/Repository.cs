using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using HotelAPI.Data.Repositories.IRepositories;

namespace HotelAPI.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _context;
        private DbSet<T> _dbSet;

        public Repository(HotelierDBConText context)
        {
            _context = context;
            _dbSet = context.Set<T>();

        }

        public ValueTask<T?> Get(int id)
        {
            return _dbSet.FindAsync(id);
        }

        public Task<List<T>> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            if (orderBy != null)
            {
                return orderBy(query).ToListAsync();
            }
            return query.ToListAsync();
        }

        public Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return query.FirstOrDefaultAsync();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }
        public void Remove(int id)
        {
            T entityToRemove = _dbSet.Find(id);
            if (entityToRemove != null)
            {
                Remove(entityToRemove);
            }
        }
        public void Remove(T Entity)
        {
            _dbSet.Remove(Entity);
        }
    }
}