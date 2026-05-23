using GastosControl.Application.Abstractions.Repositories;
using GastosControl.Application.Transactions;
using GastosControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GastosControl.Infrastructure.Persistence.Repositories;

public sealed class TransactionRepository : ITransactionRepository
{
    private readonly GastosDbContext _context;

    public TransactionRepository(GastosDbContext context)
    {
        _context = context;
    }

    public Task<FinancialTransaction?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return _context.Transactions
            .Include(transaction => transaction.Category)
            .FirstOrDefaultAsync(transaction => transaction.Id == id && transaction.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<FinancialTransaction>> ListAsync(Guid userId, TransactionListQuery query, CancellationToken cancellationToken = default)
    {
        var transactions = _context.Transactions
            .AsNoTracking()
            .Include(transaction => transaction.Category)
            .Where(transaction => transaction.UserId == userId);

        if (query.Type.HasValue)
        {
            transactions = transactions.Where(transaction => transaction.Type == query.Type.Value);
        }

        if (query.CategoryId.HasValue)
        {
            transactions = transactions.Where(transaction => transaction.CategoryId == query.CategoryId.Value);
        }

        if (query.From.HasValue)
        {
            transactions = transactions.Where(transaction => transaction.OccurredOn >= query.From.Value);
        }

        if (query.To.HasValue)
        {
            transactions = transactions.Where(transaction => transaction.OccurredOn <= query.To.Value);
        }

        return await transactions
            .OrderByDescending(transaction => transaction.OccurredOn)
            .ThenByDescending(transaction => transaction.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(FinancialTransaction transaction, CancellationToken cancellationToken = default)
    {
        await _context.Transactions.AddAsync(transaction, cancellationToken);
    }

    public void Remove(FinancialTransaction transaction)
    {
        _context.Transactions.Remove(transaction);
    }
}
