using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPQ.Client.RequestDtos
{
    public sealed class StatementRequest
    {
        public int AccountId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        // DataTables server-side
        public int Draw { get; set; }
        public int Start { get; set; }      // skip
        public int Length { get; set; }     // take
        public string? SortColumn { get; set; }
        public string? SortDir { get; set; } // asc|desc
        public string? Search { get; set; }  // global search
    }
}
