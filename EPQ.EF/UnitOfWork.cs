using EPQ.EF;
using EPQ.Domin.Models;
using EPQ.EF.Interfaces;
using EPQ.EF.Repositories;

namespace HoldPoint.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EPQContext _context;


        public IBaseRepository<Balance> Balances { get; private set; }
        public IBaseRepository<BalanceHistory> BalanceHistories { get; private set; }

        
        public UnitOfWork(EPQContext context)
        {
            _context = context;

            Balances = new BaseRepository<Balance>(_context);
            BalanceHistories = new BaseRepository<BalanceHistory>(_context);
          
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}