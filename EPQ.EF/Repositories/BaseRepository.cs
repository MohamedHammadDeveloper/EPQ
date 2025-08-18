
using EPQ.EF.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EPQ.EF.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected EPQContext _context;


        public BaseRepository(EPQContext context)
        {
            _context = context;
        }

       // List<Batches> b = new List<Batches>();
       // ArrayList b = new ArrayList();
       // Hashtable ht = new Hashtable();
       // List<T> bg = new List<T>();
        //public IEnumerable<T> GetAll()
        //{
        //    return _context.Set<T>().ToList();
        //}
        public DbSet<T> GetDbSet()
        {
            return _context.Set<T>();
        }
        //public async Task<IEnumerable<T>> GetAllAsync()
        //{
        //    return await _context.Set<T>().ToListAsync();
        //}

        public T GetById(int id) => _context.Set<T>().Find(id); // Arrow Function

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public T? GetFirst<TKey>(Expression<Func<T, TKey>> orderBySelector)
        {
            return _context.Set<T>()
                           .OrderBy(orderBySelector)
                           .FirstOrDefault();
        }

        public T? GetLast<TKey>(Expression<Func<T, TKey>> orderBySelector)
        {
            return _context.Set<T>()
                           .OrderByDescending(orderBySelector)
                           .FirstOrDefault();
        }


        public T Find(Expression<Func<T, bool>> criteria) => _context.Set<T>().SingleOrDefault(criteria);
        public T FindInclude(Expression<Func<T, bool>> criteria, Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending , string[] includes = null) {
            IQueryable<T> query = _context.Set<T>().Where(criteria);

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);
            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }

            return query.FirstOrDefault();
        }
        public T FindNoTraking(Expression<Func<T, bool>> criteria) => _context.Set<T>().Where(criteria).AsNoTracking().FirstOrDefault();
        public T FindNoTrakingInclude(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>().Where(criteria);

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return query.AsNoTracking().SingleOrDefault(criteria);
        }
        public T FindNoTraking(Expression<Func<T, bool>> criteria, Expression<Func<T, bool>> criteria2, string[] includes = null) => _context.Set<T>().Where(criteria).Where(criteria2).AsNoTracking().FirstOrDefault();

        public T FindNoTraking(Expression<Func<T, bool>> criteria, Expression<Func<T, bool>> criteria2, Expression<Func<T, bool>> criteria3, string[] includes = null) => _context.Set<T>().Where(criteria).Where(criteria2).Where(criteria3).AsNoTracking().FirstOrDefault();
        public T FindNoTrakingWithOrder(Expression<Func<T, bool>> criteria, Expression<Func<T, object>> orderBy = null) => _context.Set<T>().Where(criteria).OrderBy(orderBy).AsNoTracking().FirstOrDefault();

        public T FindNoTraking(Expression<Func<T, bool>> criteria, Expression<Func<T, bool>> criteria2,
            Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending, string[] includes = null) => _context.Set<T>().Where(criteria).Where(criteria2).OrderBy(orderBy).AsNoTracking().FirstOrDefault();

        public T Find(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return query.SingleOrDefault(criteria);
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> criteria) => await _context.Set<T>().FirstOrDefaultAsync(criteria);
        public async Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            return await query.FirstOrDefaultAsync(criteria);
        }
        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria) => _context.Set<T>().Where(criteria).ToList();

        public IEnumerable<T> FindAlike(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            return _context.Set<T>().Where(criteria).AsEnumerable();
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return query.Where(criteria).ToList();
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int skip, int take)
        {
            return _context.Set<T>().Where(criteria).Skip(skip).Take(take).ToList();
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int? skip, int? take,
            Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending)
        {
            IQueryable<T> query = _context.Set<T>().Where(criteria);
           
            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            if(orderBy != null)
            {
                if(orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }

            return query.ToList();
        }

        public IQueryable<T> FindAllAsQueryable(Expression<Func<T, bool>> criteria = null, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            // Apply eager loading for the specified navigation properties
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            // Apply the filter, if provided
            if (criteria != null)
            {
                query = query.Where(criteria);
            }

            return query;
        }
        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria) => await _context.Set<T>().Where(criteria).ToListAsync();
        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return await query.Where(criteria).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int take, int skip)
        {
            return await _context.Set<T>().Where(criteria).Skip(skip).Take(take).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int? take, int? skip,
            Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending)
        {
            IQueryable<T> query = _context.Set<T>().Where(criteria);

            if (take.HasValue)
                query = query.Take(take.Value);

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }

            return await query.ToListAsync();
        }

        public IEnumerable<T> FindAllInclude(string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return query.ToList();
        }
        public IEnumerable<T> FindAllInclude(Expression<Func<T, object>> orderBy, int skip, int take, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return query.OrderByDescending(orderBy).Skip(skip).Take(take).ToList();
        }
        public IEnumerable<T> FindAllInclude(int skip, int take, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return query.Skip(skip).Take(take).ToList();
        }
        public virtual IQueryable<T> GetAllNolist()
        {
            return _context.Set<T>().AsNoTracking();
        }
        public T Add(T entity)
        {
            //entity
            _context.Set<T>().Add(entity);
            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {

            await _context.Set<T>().AddAsync(entity);
            //entity.Property(propertyName).CurrentValue = someValue;
            return entity;
        }

        public IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
            return entities;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            return entities;
        }

        public T Update(T entity)
        {
            //_context.Set<T>().AsNoTracking().ExecuteUpdate(entity);
            _context.Update(entity);
            return entity;
        }
        public IEnumerable<T> UpdateRange(IEnumerable<T> entities)
        {
            _context.UpdateRange(entities);
            return entities;
        }
        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public void Attach(T entity)
        {
            _context.Set<T>().Attach(entity);
        }

        public void AttachRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AttachRange(entities);
        }

        public int Count()
        {
            return _context.Set<T>().Count();
        }

        public int Count(Expression<Func<T, bool>> criteria)
        {
            return _context.Set<T>().Count(criteria);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> criteria)
        {
            return await _context.Set<T>().CountAsync(criteria);
        }

        public async Task<IEnumerable<T>> GetChildsAsync(Expression<Func<T, object>> include , Expression<Func<T, bool>> criteria)
        {
            return await _context.Set<T>()
                      .Include(include)
                      .Where(criteria).ToListAsync();
        }
        public bool isExist(Expression<Func<T, bool>> criteria)
        {
            return _context.Set<T>().Any(criteria);
        }

        private const int MaxDepth = 3;
        private IQueryable<TEntity> IncludeWithDepth<TEntity>(IQueryable<TEntity> query, Type entityType, int depth = 1)
     where TEntity : class
        {
            if (depth == 0) return query;

            var navigations = _context.Model.FindEntityType(entityType)
                ?.GetNavigations()
                .Select(n => n.Name)
                .ToList();

            if (navigations == null || !navigations.Any())
                return query;

            foreach (var navigation in navigations)
            {
                query = query.Include(navigation);
                var navType = _context.Model.FindEntityType(entityType)
                    ?.FindNavigation(navigation)?.TargetEntityType.ClrType;

                if (navType != null)
                    query = IncludeWithDepth(query, navType, depth - 1);
            }

            return query;
        }




        public IEnumerable<T> GetAll()
        {
            var query = _context.Set<T>().AsQueryable();
            query = IncludeWithDepth(query, typeof(T));
            return query.ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var query = _context.Set<T>().AsQueryable();
            query = IncludeWithDepth(query, typeof(T));
            return await query.ToListAsync();
        }

         public IEnumerable<T> GetAllInclude(List<string> includes)
        {
            var query = _context.Set<T>().AsQueryable();

            //IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return query.ToList();

        }

        public async Task<IEnumerable<T>> GetAllIncludeAsync(List<string> includes)
        {
            var query = _context.Set<T>().AsQueryable();
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return await query.ToListAsync();
        }

         public IEnumerable<T> GetAllIncludeWithCriteria(List<string> includes, Expression<Func<T, bool>> criteria)
        {
            var query = _context.Set<T>().AsQueryable();

            //IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);
            query = query.Where(criteria);

            return query.ToList();

        }

        public async Task<IEnumerable<T>> GetAllIncludeWithCriteriaAsync(List<string> includes, Expression<Func<T, bool>> criteria)
        {
            var query = _context.Set<T>().AsQueryable();
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            query = query.Where(criteria);
            return await query.ToListAsync();
        }


        /*
public async Task<string> ChildCoa(string HeadName, string HeadCode, string coa)
{
   if (HeadCode == "0") { coa += "<li class=\"jstree-open \">" + HeadName + ""; }
   else { coa += "<li><a href='javascript:' onclick=\"loadCoaData('" + HeadCode + "')\">" + HeadName + "</a>  "; }

   //if (HeadCode == 0 ) coa += "<li class=\"jstree-open \">" + HeadName + "";
   List<AccCoa> selected = await _context.AccCoa.Where(x => x.PHeadCode == HeadCode).ToListAsync();
   if (selected.Count > 0)
   {
       coa += "<ul>";
       foreach (var item in selected)
       {
          // coa += "<li><a href='javascript:' onclick=\"loadCoaData('" + item.HeadCode + "')\">" + item.HeadName + "</a></li>  ";
           await ChildCoa(item.HeadName, item.HeadCode, coa);
       }
       coa += "</ul>";
   }

   return coa;
}
*/
    }
}