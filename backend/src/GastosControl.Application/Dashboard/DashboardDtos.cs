using GastosControl.Application.Transactions;
using GastosControl.Domain.Enums;

namespace GastosControl.Application.Dashboard;

public sealed record DashboardRequest(
    int? Year = null,
    int? Month = null,
    TransactionType? Type = null,
    Guid? CategoryId = null);

public sealed record DashboardDto(
    int Year,
    int Month,
    decimal MonthlyIncome,
    decimal MonthlyExpense,
    decimal MonthlyBalance,
    decimal AnnualIncome,
    decimal AnnualExpense,
    decimal AnnualBalance,
    IReadOnlyList<MonthlyTotalDto> MonthlyTotals,
    IReadOnlyList<CategoryTotalDto> CategoryTotals,
    IReadOnlyList<TransactionDto> RecentTransactions);

public sealed record MonthlyTotalDto(int Month, decimal Income, decimal Expense, decimal Balance);

public sealed record CategoryTotalDto(Guid CategoryId, string CategoryName, string CategoryColor, TransactionType Type, decimal Total);
