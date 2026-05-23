using GastosControl.Application.Transactions;
using GastosControl.Domain.Entities;

namespace GastosControl.Application.Abstractions.Repositories;

public interface ITransactionRepository
{
    Task<FinancialTransaction?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FinancialTransaction>> ListAsync(Guid userId, TransactionListQuery query, CancellationToken cancellationToken = default);

    Task AddAsync(FinancialTransaction transaction, CancellationToken cancellationToken = default);

    void Remove(FinancialTransaction transaction);
}
