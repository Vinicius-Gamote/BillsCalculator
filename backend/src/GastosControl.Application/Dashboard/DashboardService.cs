using GastosControl.Application.Abstractions.Repositories;
using GastosControl.Application.Common;
using GastosControl.Application.Transactions;
using GastosControl.Domain.Enums;

namespace GastosControl.Application.Dashboard;

public sealed class DashboardService
{
    private readonly ITransactionRepository _transactions;
    private readonly IUserRepository _users;

    public DashboardService(ITransactionRepository transactions, IUserRepository users)
    {
        _transactions = transactions;
        _users = users;
    }

    public async Task<DashboardDto> GetAsync(Guid userId, DashboardRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _users.ExistsAsync(userId, cancellationToken))
        {
            throw new UnauthorizedAppException("Usuario invalido.");
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        var year = request.Year ?? today.Year;
        var month = request.Month ?? today.Month;

        if (month < 1 || month > 12)
        {
            throw new ValidationAppException("Informe um mes entre 1 e 12.");
        }

        var query = new TransactionListQuery(
            request.Type,
            request.CategoryId,
            new DateOnly(year, 1, 1),
            new DateOnly(year, 12, 31));

        var annualTransactions = await _transactions.ListAsync(userId, query, cancellationToken);
        var monthlyTransactions = annualTransactions.Where(transaction => transaction.OccurredOn.Month == month).ToList();

        var annualIncome = SumByType(annualTransactions, TransactionType.Income);
        var annualExpense = SumByType(annualTransactions, TransactionType.Expense);
        var monthlyIncome = SumByType(monthlyTransactions, TransactionType.Income);
        var monthlyExpense = SumByType(monthlyTransactions, TransactionType.Expense);

        var monthlyTotals = Enumerable.Range(1, 12)
            .Select(currentMonth =>
            {
                var items = annualTransactions.Where(transaction => transaction.OccurredOn.Month == currentMonth).ToList();
                var income = SumByType(items, TransactionType.Income);
                var expense = SumByType(items, TransactionType.Expense);
                return new MonthlyTotalDto(currentMonth, income, expense, income - expense);
            })
            .ToList();

        var categoryTotals = annualTransactions
            .GroupBy(transaction => new
            {
                transaction.CategoryId,
                CategoryName = transaction.Category?.Name ?? "Sem categoria",
                CategoryColor = transaction.Category?.Color ?? "#64748b",
                transaction.Type
            })
            .Select(group => new CategoryTotalDto(
                group.Key.CategoryId,
                group.Key.CategoryName,
                group.Key.CategoryColor,
                group.Key.Type,
                group.Sum(transaction => transaction.Amount)))
            .OrderByDescending(total => total.Total)
            .ToList();

        var recentTransactions = annualTransactions
            .OrderByDescending(transaction => transaction.OccurredOn)
            .ThenByDescending(transaction => transaction.CreatedAt)
            .Take(8)
            .Select(TransactionService.Map)
            .ToList();

        return new DashboardDto(
            year,
            month,
            monthlyIncome,
            monthlyExpense,
            monthlyIncome - monthlyExpense,
            annualIncome,
            annualExpense,
            annualIncome - annualExpense,
            monthlyTotals,
            categoryTotals,
            recentTransactions);
    }

    private static decimal SumByType(IEnumerable<Domain.Entities.FinancialTransaction> transactions, TransactionType type)
    {
        return transactions.Where(transaction => transaction.Type == type).Sum(transaction => transaction.Amount);
    }
}
