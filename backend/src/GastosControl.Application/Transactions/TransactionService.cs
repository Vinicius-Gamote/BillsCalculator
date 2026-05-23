using GastosControl.Application.Abstractions;
using GastosControl.Application.Abstractions.Repositories;
using GastosControl.Application.Common;
using GastosControl.Domain.Entities;

namespace GastosControl.Application.Transactions;

public sealed class TransactionService
{
    private readonly ITransactionRepository _transactions;
    private readonly ICategoryRepository _categories;
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionService(
        ITransactionRepository transactions,
        ICategoryRepository categories,
        IUserRepository users,
        IUnitOfWork unitOfWork)
    {
        _transactions = transactions;
        _categories = categories;
        _users = users;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<TransactionDto>> ListAsync(Guid userId, TransactionListQuery query, CancellationToken cancellationToken = default)
    {
        await EnsureUserExistsAsync(userId, cancellationToken);

        var transactions = await _transactions.ListAsync(userId, query, cancellationToken);
        return transactions.Select(Map).ToList();
    }

    public async Task<TransactionDto> CreateAsync(Guid userId, CreateTransactionRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureUserExistsAsync(userId, cancellationToken);
        await EnsureCategoryCanBeUsedAsync(userId, request.CategoryId, request.Type, cancellationToken);

        var transaction = new FinancialTransaction(
            userId,
            request.CategoryId,
            request.Type,
            request.Description,
            request.Amount,
            request.OccurredOn,
            request.Notes);

        await _transactions.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _transactions.GetByIdForUserAsync(transaction.Id, userId, cancellationToken);
        return Map(created ?? transaction);
    }

    public async Task<TransactionDto> UpdateAsync(Guid userId, Guid id, UpdateTransactionRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureCategoryCanBeUsedAsync(userId, request.CategoryId, request.Type, cancellationToken);

        var transaction = await _transactions.GetByIdForUserAsync(id, userId, cancellationToken)
            ?? throw new NotFoundAppException("Lancamento nao encontrado.");

        transaction.Update(request.CategoryId, request.Type, request.Description, request.Amount, request.OccurredOn, request.Notes);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _transactions.GetByIdForUserAsync(transaction.Id, userId, cancellationToken);
        return Map(updated ?? transaction);
    }

    public async Task DeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        var transaction = await _transactions.GetByIdForUserAsync(id, userId, cancellationToken)
            ?? throw new NotFoundAppException("Lancamento nao encontrado.");

        _transactions.Remove(transaction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureUserExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        if (!await _users.ExistsAsync(userId, cancellationToken))
        {
            throw new UnauthorizedAppException("Usuario invalido.");
        }
    }

    private async Task EnsureCategoryCanBeUsedAsync(Guid userId, Guid categoryId, Domain.Enums.TransactionType type, CancellationToken cancellationToken)
    {
        var category = await _categories.GetByIdForUserAsync(categoryId, userId, cancellationToken)
            ?? throw new ValidationAppException("Categoria invalida.");

        if (category.IsArchived)
        {
            throw new ValidationAppException("A categoria esta arquivada.");
        }

        if (category.Type != type)
        {
            throw new ValidationAppException("O tipo do lancamento deve ser igual ao tipo da categoria.");
        }
    }

    internal static TransactionDto Map(FinancialTransaction transaction)
    {
        return new TransactionDto(
            transaction.Id,
            transaction.CategoryId,
            transaction.Category?.Name ?? "Sem categoria",
            transaction.Category?.Color ?? "#64748b",
            transaction.Type,
            transaction.Description,
            transaction.Amount,
            transaction.OccurredOn,
            transaction.Notes);
    }
}
