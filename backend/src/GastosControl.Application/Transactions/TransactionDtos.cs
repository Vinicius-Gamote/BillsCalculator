using GastosControl.Domain.Enums;

namespace GastosControl.Application.Transactions;

public sealed record TransactionDto(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    string CategoryColor,
    TransactionType Type,
    string Description,
    decimal Amount,
    DateOnly OccurredOn,
    string? Notes);

public sealed record CreateTransactionRequest(
    Guid CategoryId,
    TransactionType Type,
    string Description,
    decimal Amount,
    DateOnly OccurredOn,
    string? Notes);

public sealed record UpdateTransactionRequest(
    Guid CategoryId,
    TransactionType Type,
    string Description,
    decimal Amount,
    DateOnly OccurredOn,
    string? Notes);

public sealed record TransactionListQuery(
    TransactionType? Type = null,
    Guid? CategoryId = null,
    DateOnly? From = null,
    DateOnly? To = null);
