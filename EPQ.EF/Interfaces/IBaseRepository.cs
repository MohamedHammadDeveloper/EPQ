using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public static class OrderBy
{
    public const string Ascending = "ASC";
    public const string Descending = "DESC";
}

namespace EPQ.EF.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        T? GetFirst<TKey>(Expression<Func<T, TKey>> orderBySelector);
        T? GetLast<TKey>(Expression<Func<T, TKey>> orderBySelector);
        T GetById(int id);
        Task<T> GetByIdAsync(int id);
        IEnumerable<T> GetAll();
        DbSet<T> GetDbSet();
        Task<IEnumerable<T>> GetAllAsync();
        T Find(Expression<Func<T, bool>> criteria, string[] includes = null);
        T FindInclude(Expression<Func<T, bool>> criteria, Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending, string[] includes = null);
        Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includes = null);
        T FindNoTrakingInclude(Expression<Func<T, bool>> criteria, string[] includes = null);
        T FindNoTraking(Expression<Func<T, bool>> criteria, Expression<Func<T, bool>> criteria2, string[] includes = null);
        T FindNoTraking(Expression<Func<T, bool>> criteria, Expression<Func<T, bool>> criteria2, Expression<Func<T, bool>> criteria3, string[] includes = null);
        T FindNoTrakingWithOrder(Expression<Func<T, bool>> criteria, Expression<Func<T, object>> orderBy = null);
        T FindNoTraking(Expression<Func<T, bool>> criteria);

        T FindNoTraking(Expression<Func<T, bool>> criteria, Expression<Func<T, bool>> criteria2,
            Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending, string[] includes = null);

        IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, string[] includes = null);
        IEnumerable<T> FindAlike(Expression<Func<T, bool>> criteria, string[] includes = null);
      
        IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int take, int skip);
        IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int? take, int? skip, 
            Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending);
        IQueryable<T> FindAllAsQueryable(Expression<Func<T, bool>> criteria, string[] includes = null);

        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int skip, int take);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int? skip, int? take,
            Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending);
        IEnumerable<T> FindAllInclude(string[] includes = null);
        IEnumerable<T> FindAllInclude(int skip, int take, string[] includes = null);
        IEnumerable<T> FindAllInclude(Expression<Func<T, object>> orderBy, int skip, int take, string[] includes = null);
        IQueryable<T> GetAllNolist();
        T Add(T entity);
        Task<T> AddAsync(T entity);
        IEnumerable<T> AddRange(IEnumerable<T> entities);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        T Update(T entity);
        //Task<T> UpdateAsync(T entity);
        IEnumerable<T> UpdateRange(IEnumerable<T> entities);
        //Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        void Attach(T entity);
        void AttachRange(IEnumerable<T> entities);
        int Count();
        int Count(Expression<Func<T, bool>> criteria);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> criteria);

        Task<IEnumerable<T>> GetChildsAsync(Expression<Func<T, object>> include, Expression<Func<T, bool>> criteria);

        bool isExist(Expression<Func<T,bool>> criteria);

        IEnumerable<T> GetAllInclude(List<string> includes);
        Task<IEnumerable<T>> GetAllIncludeAsync(List<string> includes);
        IEnumerable<T> GetAllIncludeWithCriteria(List<string> includes, Expression<Func<T, bool>> criteria);
        Task<IEnumerable<T>> GetAllIncludeWithCriteriaAsync(List<string> includes, Expression<Func<T, bool>> criteria);

        //Task<string> ChildCoa(string HeadName, string HeadCode, string coa);
    }
}