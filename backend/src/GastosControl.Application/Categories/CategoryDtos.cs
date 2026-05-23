using GastosControl.Domain.Enums;

namespace GastosControl.Application.Categories;

public sealed record CategoryDto(Guid Id, string Name, TransactionType Type, string Color, bool IsArchived);

public sealed record CreateCategoryRequest(string Name, TransactionType Type, string Color);

public sealed record UpdateCategoryRequest(string Name, TransactionType Type, string Color);
