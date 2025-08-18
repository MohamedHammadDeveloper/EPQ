using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPQ.Client.RequestDtos
{
    public sealed class PagedResult<T>
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
        public StatementTotalsDto Totals { get; set; } = new();
    }
}
