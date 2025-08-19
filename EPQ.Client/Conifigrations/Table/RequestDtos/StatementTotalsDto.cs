using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPQ.Client.RequestDtos
{
    public sealed class StatementTotalsDto
    {
        public decimal TotalDebits { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal AccountFinalBalance { get; set; }     
        public decimal FirstPreviousBalance { get; set; }    
    }
}
