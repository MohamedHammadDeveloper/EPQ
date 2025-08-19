using EPQ.Client.Conifigrations.Table.Interfaces;
using EPQ.Client.RequestDtos;
using EPQ.Domin.Models;
using EPQ.EF.Interfaces;
using EPQ.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;

namespace EPQ.Client.Conifigrations.Table
{
    public sealed class AccountStatementService : IAccountStatementService
    {
        private readonly IUnitOfWork _uow;

        public AccountStatementService(IUnitOfWork uow) => _uow = uow;

        public async Task<PagedResult<StatementRowDto>> GetStatementAsync(StatementRequest req, CancellationToken ct = default)
        {
            // Get Account By AccountId
            var account = await _uow.Balances.GetDbSet()
                .AsNoTracking()
                .Where(b => b.BalanceId == req.AccountId)
                .Select(b => new { b.BalanceId, b.BalanceName, b.BalanceType })
                .FirstOrDefaultAsync(ct);

            if (account is null)
                return new PagedResult<StatementRowDto> { Draw = req.Draw, RecordsTotal = 0, RecordsFiltered = 0 };

            bool isDebitType = string.Equals(account.BalanceType, "debit", StringComparison.OrdinalIgnoreCase);

            var hist = _uow.BalanceHistories.GetDbSet().AsNoTracking()
                .Where(h => h.BalanceId == req.AccountId
                         && h.Date >= req.DateFrom
                         && h.Date <= req.DateTo);

            // Global search
            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                string s = req.Search.Trim();
                hist = hist.Where(h =>
                    h.Remarks!.Contains(s) ||
                    h.Debtor.ToString().Contains(s) ||
                    h.Creditor.ToString().Contains(s) ||
                    h.PrevBalnce.ToString().Contains(s));
            }

            
            int recordsFiltered = await hist.CountAsync(ct);

            // Totals
            var totalsProjection = await hist
                .OrderBy(h => h.Date)
                .Select(h => new { h.Debtor, h.Creditor, h.PrevBalnce })
                .ToListAsync(ct);

            decimal totalDebits = totalsProjection.Sum(x => (decimal)(x.Debtor ?? 0m));
            decimal totalCredits = totalsProjection.Sum(x => (decimal)(x.Creditor ?? 0m));
            decimal firstPrev = totalsProjection.FirstOrDefault()?.PrevBalnce ?? 0m;

            //Get Last Record before sorting
            decimal lastFinal = 0m;
            if (totalsProjection.Count() > 0)
            {
                var last = totalsProjection[^1];
                var prev = (decimal)(last.PrevBalnce ?? 0m);
                var deb = (decimal)(last.Debtor ?? 0m);
                var cre = (decimal)(last.Creditor ?? 0m);
                lastFinal = isDebitType ? ((prev + deb) - cre) : ((prev + cre) - deb);
            }

            // Sorting 
            IOrderedQueryable<BalanceHistory> ordered = (req.SortColumn, req.SortDir?.ToLower()) switch
            {
                ("AccountId", "desc") => hist.OrderByDescending(h => h.BalanceId),
                ("AccountId", _) => hist.OrderBy(h => h.BalanceId),

                ("PreviousBalance", "desc") => hist.OrderByDescending(h => h.PrevBalnce),
                ("PreviousBalance", _) => hist.OrderBy(h => h.PrevBalnce),

                ("Debit", "desc") => hist.OrderByDescending(h => h.Debtor),
                ("Debit", _) => hist.OrderBy(h => h.Debtor),

                ("Credit", "desc") => hist.OrderByDescending(h => h.Creditor),
                ("Credit", _) => hist.OrderBy(h => h.Creditor),

                ("FinalBalance", "desc") => hist.OrderByDescending(h =>
                    isDebitType
                        ? ((decimal)(h.PrevBalnce ?? 0) + (decimal)(h.Debtor ?? 0) - (decimal)(h.Creditor ?? 0))
                        : ((decimal)(h.PrevBalnce ?? 0) + (decimal)(h.Creditor ?? 0) - (decimal)(h.Debtor ?? 0))
                ),
                ("FinalBalance", _) => hist.OrderBy(h =>
                    isDebitType
                        ? ((decimal)(h.PrevBalnce ?? 0) + (decimal)(h.Debtor ?? 0) - (decimal)(h.Creditor ?? 0))
                        : ((decimal)(h.PrevBalnce ?? 0) + (decimal)(h.Creditor ?? 0) - (decimal)(h.Debtor ?? 0))
                ),

                ("TxnDate", "desc") => hist.OrderByDescending(h => h.Date),
                _ => hist.OrderBy(h => h.Date) // default
            };

            // Paging
            var page = await ordered
                .Skip(req.Start)
                .Take(req.Length)
                .Select(h => new StatementRowDto
                {
                    BalanceHistoryId = h.BalanceHisId,
                    AccountId = h.BalanceId??0,
                    AccountName = account.BalanceName ?? "",
                    TxnDate = h.Date??DateTime.Now,
                    PreviousBalance = (decimal)(h.PrevBalnce ?? 0m),
                    Debit = (decimal)(h.Debtor ?? 0m),
                    Credit = (decimal)(h.Creditor ?? 0m),
                    FinalBalance = isDebitType
                        ? ((decimal)(h.PrevBalnce ?? 0m) + (decimal)(h.Debtor ?? 0m) - (decimal)(h.Creditor ?? 0m))
                        : ((decimal)(h.PrevBalnce ?? 0m) + (decimal)(h.Creditor ?? 0m) - (decimal)(h.Debtor ?? 0m)),
                    Description = h.Remarks
                })
                .ToListAsync(ct);

          
            var dt =  new PagedResult<StatementRowDto>
            {
                Draw = req.Draw,
                RecordsTotal = recordsFiltered,
                RecordsFiltered = recordsFiltered,
                Data = page,
                Totals = new StatementTotalsDto
                {
                    TotalDebits = totalDebits,
                    TotalCredits = totalCredits,
                    FirstPreviousBalance = firstPrev,
                    AccountFinalBalance = lastFinal
                }
            };

            return dt;
        }


        // Details Of Balance_History using BalanceHisId
        public async Task<StatementRowDto?> GetTxDetailsAsync(int id, CancellationToken ct = default)
        {
            var h = _uow.BalanceHistories.Find(x => x.BalanceHisId == id);
            var b = _uow.Balances.Find(x => x.BalanceId == h.BalanceId);
                   
            if (h is null) return null;

            bool isDebitType = string.Equals(b.BalanceType, "debit", StringComparison.OrdinalIgnoreCase);
            decimal prev = (decimal)(h.PrevBalnce ?? 0m);
            decimal deb = (decimal)(h.Debtor ?? 0m);
            decimal cre = (decimal)(h.Creditor ?? 0m);

            var dt = new StatementRowDto
            {
                BalanceHistoryId = h.BalanceHisId,
                AccountId = h.BalanceId??0,
                AccountName = b.BalanceName ?? "",
                TxnDate = h.Date ?? DateTime.Now,
                PreviousBalance = prev,
                Debit = deb,
                Credit = cre,
                FinalBalance = isDebitType ? ((prev + deb) - cre) : ((prev + cre) - deb),
                Description = h.Remarks.Trim()
                        
        };

            return dt;
        }
    }
}
