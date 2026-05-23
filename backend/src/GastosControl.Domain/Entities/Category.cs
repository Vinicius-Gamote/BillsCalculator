using GastosControl.Domain.Common;
using GastosControl.Domain.Enums;
using GastosControl.Domain.Exceptions;

namespace GastosControl.Domain.Entities;

public sealed class Category : Entity
{
    private readonly List<FinancialTransaction> _transactions = [];

    private Category()
    {
        Name = string.Empty;
        Color = string.Empty;
    }

    public Category(Guid userId, string name, TransactionType type, string color)
        : base(Guid.NewGuid())
    {
        UserId = userId == Guid.Empty ? throw new DomainException("Usuario invalido.") : userId;
        Type = type;
        CreatedAt = DateTimeOffset.UtcNow;
        Update(name, type, color);
    }

    public Guid UserId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public TransactionType Type { get; private set; }

    public string Color { get; private set; } = string.Empty;

    public bool IsArchived { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public User? User { get; private set; }

    public IReadOnlyCollection<FinancialTransaction> Transactions => _transactions;

    public void Update(string name, TransactionType type, string color)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("O nome da categoria e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(color))
        {
            color = "#2563eb";
        }

        Name = name.Trim();
        Type = type;
        Color = color.Trim();
    }

    public void Archive()
    {
        IsArchived = true;
    }

    public void Restore()
    {
        IsArchived = false;
    }
}
