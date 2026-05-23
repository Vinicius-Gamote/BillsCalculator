using GastosControl.Application.Abstractions.Repositories;
using GastosControl.Domain.Entities;
using GastosControl.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GastosControl.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly GastosDbContext _context;

    public CategoryRepository(GastosDbContext context)
    {
        _context = context;
    }

    public Task<Category?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return _context.Categories.FirstOrDefaultAsync(category => category.Id == id && category.UserId == userId, cancellationToken);
    }

    public Task<Category?> GetByNameForUserAsync(string name, Guid userId, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim();
        return _context.Categories.FirstOrDefaultAsync(
            category => category.UserId == userId && category.Name == normalizedName,
            cancellationToken);
    }

    public Task<bool> ExistsByNameForUserAsync(string name, Guid userId, Guid? ignoreId = null, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim();
        return _context.Categories.AsNoTracking().AnyAsync(
            category => category.UserId == userId
                && category.Name == normalizedName
                && (!ignoreId.HasValue || category.Id != ignoreId.Value),
            cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> ListAsync(Guid userId, TransactionType? type = null, bool includeArchived = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Categories
            .AsNoTracking()
            .Where(category => category.UserId == userId);

        if (type.HasValue)
        {
            query = query.Where(category => category.Type == type.Value);
        }

        if (!includeArchived)
        {
            query = query.Where(category => !category.IsArchived);
        }

        return await query
            .OrderBy(category => category.Type)
            .ThenBy(category => category.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        await _context.Categories.AddAsync(category, cancellationToken);
    }
}
