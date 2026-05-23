using GastosControl.Api.Extensions;
using GastosControl.Application.Categories;
using GastosControl.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GastosControl.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoriesController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> List([FromQuery] TransactionType? type, CancellationToken cancellationToken)
    {
        return Ok(await _categoryService.ListAsync(User.GetUserId(), type, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _categoryService.CreateAsync(User.GetUserId(), request, cancellationToken));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> Update(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _categoryService.UpdateAsync(User.GetUserId(), id, request, cancellationToken));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Archive(Guid id, CancellationToken cancellationToken)
    {
        await _categoryService.ArchiveAsync(User.GetUserId(), id, cancellationToken);
        return NoContent();
    }
}
