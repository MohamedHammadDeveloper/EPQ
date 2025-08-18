using EPQ.Domin.Models;

namespace EPQ.EF.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<Balance> Balances { get; }
        IBaseRepository<BalanceHistory> BalanceHistories { get; }
      

        int Complete();
        Task<int> CompleteAsync();
    }
}