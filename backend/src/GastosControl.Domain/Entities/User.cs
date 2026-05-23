using GastosControl.Domain.Common;
using GastosControl.Domain.Exceptions;

namespace GastosControl.Domain.Entities;

public sealed class User : Entity
{
    private readonly List<Category> _categories = [];
    private readonly List<FinancialTransaction> _transactions = [];

    private User()
    {
        Name = string.Empty;
        Email = string.Empty;
        PasswordHash = string.Empty;
    }

    public User(string name, string email, string passwordHash)
        : base(Guid.NewGuid())
    {
        SetName(name);
        SetEmail(email);
        SetPasswordHash(passwordHash);
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    public IReadOnlyCollection<Category> Categories => _categories;

    public IReadOnlyCollection<FinancialTransaction> Transactions => _transactions;

    public void UpdateProfile(string name)
    {
        SetName(name);
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("O nome do usuario e obrigatorio.");
        }

        Name = name.Trim();
    }

    private void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@', StringComparison.Ordinal))
        {
            throw new DomainException("Informe um email valido.");
        }

        Email = email.Trim().ToLowerInvariant();
    }

    private void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainException("A senha criptografada e obrigatoria.");
        }

        PasswordHash = passwordHash;
    }
}
