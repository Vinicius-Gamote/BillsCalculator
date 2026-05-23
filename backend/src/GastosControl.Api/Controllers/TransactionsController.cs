using GastosControl.Api.Extensions;
using GastosControl.Application.Transactions;
using GastosControl.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GastosControl.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/transactions")]
public sealed class TransactionsController : ControllerBase
{
    private readonly TransactionService _transactionService;

    public TransactionsController(TransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TransactionDto>>> List(
        [FromQuery] TransactionType? type,
        [FromQuery] Guid? categoryId,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        CancellationToken cancellationToken)
    {
        var query = new TransactionListQuery(type, categoryId, from, to);
        return Ok(await _transactionService.ListAsync(User.GetUserId(), query, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Create(CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _transactionService.CreateAsync(User.GetUserId(), request, cancellationToken));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TransactionDto>> Update(Guid id, UpdateTransactionRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _transactionService.UpdateAsync(User.GetUserId(), id, request, cancellationToken));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _transactionService.DeleteAsync(User.GetUserId(), id, cancellationToken);
        return NoContent();
    }
}
