using EPQ.Client.RequestDtos;

namespace EPQ.Client.Conifigrations.Table.Interfaces
{
    public interface IAccountStatementService
    {
        Task<PagedResult<StatementRowDto>> GetStatementAsync(StatementRequest req, CancellationToken ct = default);
        Task<StatementRowDto?> GetTxDetailsAsync(int balanceHistoryId, CancellationToken ct = default);
    }
}
