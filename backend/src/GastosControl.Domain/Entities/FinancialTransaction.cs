using GastosControl.Domain.Common;
using GastosControl.Domain.Enums;
using GastosControl.Domain.Exceptions;

namespace GastosControl.Domain.Entities;

public sealed class FinancialTransaction : Entity
{
    private FinancialTransaction()
    {
        Description = string.Empty;
    }

    public FinancialTransaction(
        Guid userId,
        Guid categoryId,
        TransactionType type,
        string description,
        decimal amount,
        DateOnly occurredOn,
        string? notes)
        : base(Guid.NewGuid())
    {
        UserId = userId == Guid.Empty ? throw new DomainException("Usuario invalido.") : userId;
        CategoryId = categoryId == Guid.Empty ? throw new DomainException("Categoria invalida.") : categoryId;
        CreatedAt = DateTimeOffset.UtcNow;
        Update(categoryId, type, description, amount, occurredOn, notes);
    }

    public Guid UserId { get; private set; }

    public Guid CategoryId { get; private set; }

    public TransactionType Type { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public decimal Amount { get; private set; }

    public DateOnly OccurredOn { get; private set; }

    public string? Notes { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public User? User { get; private set; }

    public Category? Category { get; private set; }

    public void Update(
        Guid categoryId,
        TransactionType type,
        string description,
        decimal amount,
        DateOnly occurredOn,
        string? notes)
    {
        if (categoryId == Guid.Empty)
        {
            throw new DomainException("Categoria invalida.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("A descricao do lancamento e obrigatoria.");
        }

        if (amount <= 0)
        {
            throw new DomainException("O valor deve ser maior que zero.");
        }

        CategoryId = categoryId;
        Type = type;
        Description = description.Trim();
        Amount = decimal.Round(amount, 2);
        OccurredOn = occurredOn;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }
}
