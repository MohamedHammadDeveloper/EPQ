using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPQ.Client.RequestDtos
{
    public sealed class StatementRowDto
    {
        public long BalanceHistoryId { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; } = "";
        public DateTime TxnDate { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal FinalBalance { get; set; }
        public string? Description { get; set; }
    }
}
