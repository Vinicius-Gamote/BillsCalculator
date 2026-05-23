using GastosControl.Domain.Entities;
using GastosControl.Domain.Enums;

namespace GastosControl.Application.Abstractions.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<Category?> GetByNameForUserAsync(string name, Guid userId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameForUserAsync(string name, Guid userId, Guid? ignoreId = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Category>> ListAsync(Guid userId, TransactionType? type = null, bool includeArchived = false, CancellationToken cancellationToken = default);

    Task AddAsync(Category category, CancellationToken cancellationToken = default);
}
