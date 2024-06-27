using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ProjektP4;

public class EmployeeExpenses
{
    public int EmployeeExpenseId { get; set; }
    public int DelegationId { get; set; }
    public int EmployeeId { get; set; }
    public decimal Cost { get; set; }
    public int CurrencyId { get; set; }
    public int TypeId { get; set; }
}

public class EmployeeExpensesContext : DbContext
{
    String connectionString = ConfigurationManager.ConnectionStrings["CompanyDelegations"].ConnectionString;
    public DbSet<EmployeeExpenses> EmployeeExpenses { get; set; }
    public DbSet<Delegations> Delegations { get; set; }
    public DbSet<CurrencyValues> CurrencyValues { get; set; }
    public DbSet<Employees> Employees { get; set; }
    public DbSet<ExpenseTypes> ExpenseTypes { get; set; }

    public IEnumerable<EmployeeExpenses> GetFiltered(int delegationId)
    {
        return EmployeeExpenses.Where(e => e.DelegationId == delegationId);
    }

    public void UpdateExpense(int expenseId,
        int employeeId,
        decimal cost,
        int typeId,
        int currencyId)
    {
        EmployeeExpenses expense = EmployeeExpenses.Find(expenseId);
        expense.EmployeeId = employeeId;
        expense.Cost = cost;
        expense.TypeId = typeId;
        expense.CurrencyId = currencyId;
        SaveChanges();
    }

    public void AddExpense(int delegationId,
        int employeeId,
        decimal cost,
        int typeId, int currencyId)
    {
        EmployeeExpenses expense = new EmployeeExpenses();
        expense.EmployeeId = employeeId;
        expense.Cost = cost;
        expense.TypeId = typeId;
        expense.CurrencyId = currencyId;
        expense.DelegationId = delegationId;
        EmployeeExpenses.Add(expense);
        SaveChanges();
    }

    public void DeleteExpense(int expenseId)
    {
        EmployeeExpenses expense = EmployeeExpenses.Find(expenseId);
        EmployeeExpenses.Remove(expense);
        SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmployeeExpenses>(cv =>
        {
            cv.HasKey(["EmployeeExpenseId"]);
            /*cv.HasOne(d => d.currencyId).WithMany().HasForeignKey(d => d.currencyId);
cv.HasOne(d => d.typeId).WithMany().HasForeignKey(d => d.typeId);*/
        });
        modelBuilder.Entity<Delegations>(cv => { cv.HasKey(["delegationId"]); });
        modelBuilder.Entity<Employees>(cv => { cv.HasKey(["employeeId"]); });
        modelBuilder.Entity<CurrencyValues>(cv => { cv.HasKey(["currencyValueId"]); });
        modelBuilder.Entity<ExpenseTypes>(cv => { cv.HasKey(["expenseTypeId"]); });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlServer(connectionString);
}