using EPQ.EF.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EPQ.Client.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public AccountsController(IUnitOfWork uow) => _uow = uow;

        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup([FromQuery] string q)
        {
            q ??= "";
            var data = await _uow.Balances.GetDbSet().AsNoTracking()
                .Where(b => (b.BalanceName ?? "").Contains(q) || b.BalanceId.ToString().Contains(q))
                .OrderBy(b => b.BalanceId)
                .Take(20)
                .Select(b => new {
                    id = b.BalanceId,
                    text = b.BalanceId + " - " + (b.BalanceName ?? "")
                })
                .ToListAsync();

            return Ok(data);
        }
    }
}
