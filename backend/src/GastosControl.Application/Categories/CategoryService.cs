using GastosControl.Application.Abstractions;
using GastosControl.Application.Abstractions.Repositories;
using GastosControl.Application.Common;
using GastosControl.Domain.Entities;
using GastosControl.Domain.Enums;

namespace GastosControl.Application.Categories;

public sealed class CategoryService
{
    private readonly ICategoryRepository _categories;
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(ICategoryRepository categories, IUserRepository users, IUnitOfWork unitOfWork)
    {
        _categories = categories;
        _users = users;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<CategoryDto>> ListAsync(Guid userId, TransactionType? type, CancellationToken cancellationToken = default)
    {
        await EnsureUserExistsAsync(userId, cancellationToken);

        var categories = await _categories.ListAsync(userId, type, includeArchived: false, cancellationToken);
        return categories.Select(Map).ToList();
    }

    public async Task<CategoryDto> CreateAsync(Guid userId, CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureUserExistsAsync(userId, cancellationToken);
        ValidateName(request.Name);

        var existingCategory = await _categories.GetByNameForUserAsync(request.Name, userId, cancellationToken);
        if (existingCategory is not null && !existingCategory.IsArchived)
        {
            throw new ConflictAppException("Ja existe uma categoria ativa com este nome.");
        }

        if (existingCategory is not null)
        {
            existingCategory.Update(request.Name, request.Type, request.Color);
            existingCategory.Restore();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Map(existingCategory);
        }

        var category = new Category(userId, request.Name, request.Type, request.Color);
        await _categories.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(category);
    }

    public async Task<CategoryDto> UpdateAsync(Guid userId, Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        ValidateName(request.Name);

        var category = await _categories.GetByIdForUserAsync(id, userId, cancellationToken)
            ?? throw new NotFoundAppException("Categoria nao encontrada.");

        if (await _categories.ExistsByNameForUserAsync(request.Name, userId, id, cancellationToken))
        {
            throw new ConflictAppException("Ja existe outra categoria com este nome.");
        }

        category.Update(request.Name, request.Type, request.Color);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(category);
    }

    public async Task ArchiveAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categories.GetByIdForUserAsync(id, userId, cancellationToken)
            ?? throw new NotFoundAppException("Categoria nao encontrada.");

        category.Archive();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureUserExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        if (!await _users.ExistsAsync(userId, cancellationToken))
        {
            throw new UnauthorizedAppException("Usuario invalido.");
        }
    }

    private static CategoryDto Map(Category category)
    {
        return new CategoryDto(category.Id, category.Name, category.Type, category.Color, category.IsArchived);
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationAppException("Informe o nome da categoria.");
        }
    }
}
