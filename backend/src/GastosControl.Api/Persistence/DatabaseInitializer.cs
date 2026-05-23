using GastosControl.Application.Abstractions.Security;
using GastosControl.Domain.Entities;
using GastosControl.Domain.Enums;
using GastosControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GastosControl.Api.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, IWebHostEnvironment environment)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GastosDbContext>();

        await EnsureDatabaseAsync(context);

        if (environment.IsDevelopment())
        {
            await SeedDemoDataAsync(scope.ServiceProvider, context);
        }
    }

    private static async Task EnsureDatabaseAsync(GastosDbContext context)
    {
        const int maxAttempts = 12;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await context.Database.EnsureCreatedAsync();
                return;
            }
            catch when (attempt < maxAttempts)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }

    private static async Task SeedDemoDataAsync(IServiceProvider services, GastosDbContext context)
    {
        if (await context.Users.AnyAsync())
        {
            return;
        }

        var passwordHasher = services.GetRequiredService<IPasswordHasher>();
        var user = new User("Demo Controle", "demo@gastos.local", passwordHasher.Hash("Demo@123"));

        var salary = new Category(user.Id, "Salario", TransactionType.Income, "#16a34a");
        var freelance = new Category(user.Id, "Freelance", TransactionType.Income, "#0891b2");
        var home = new Category(user.Id, "Casa", TransactionType.Expense, "#f97316");
        var food = new Category(user.Id, "Alimentacao", TransactionType.Expense, "#ef4444");
        var transport = new Category(user.Id, "Transporte", TransactionType.Expense, "#7c3aed");

        var today = DateOnly.FromDateTime(DateTime.Today);
        var transactions = new[]
        {
            new FinancialTransaction(user.Id, salary.Id, TransactionType.Income, "Salario mensal", 6200m, today.AddDays(-18), null),
            new FinancialTransaction(user.Id, freelance.Id, TransactionType.Income, "Projeto extra", 1450m, today.AddDays(-8), null),
            new FinancialTransaction(user.Id, home.Id, TransactionType.Expense, "Aluguel", 2100m, today.AddDays(-15), null),
            new FinancialTransaction(user.Id, food.Id, TransactionType.Expense, "Mercado", 780m, today.AddDays(-10), null),
            new FinancialTransaction(user.Id, transport.Id, TransactionType.Expense, "Combustivel", 360m, today.AddDays(-4), null),
            new FinancialTransaction(user.Id, salary.Id, TransactionType.Income, "Salario mes anterior", 6200m, today.AddMonths(-1).AddDays(-16), null),
            new FinancialTransaction(user.Id, food.Id, TransactionType.Expense, "Restaurantes", 520m, today.AddMonths(-1).AddDays(-3), null)
        };

        await context.Users.AddAsync(user);
        await context.Categories.AddRangeAsync(salary, freelance, home, food, transport);
        await context.Transactions.AddRangeAsync(transactions);
        await context.SaveChangesAsync();
    }
}
