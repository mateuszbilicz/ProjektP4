using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ProjektP4;

public class ExpenseTypes
{
    public int expenseTypeId { get; set; }
    public string name { get; set; }
    public double returns { get; set; }
    public bool isActive { get; set; }
}

public class ExpenseTypesContext : DbContext
{
    String connectionString = ConfigurationManager.ConnectionStrings["CompanyDelegations"].ConnectionString;
    public DbSet<ExpenseTypes> ExpenseTypes { get; set; }

    public IEnumerable<ExpenseTypes> GetFiltered(string filter)
    {
        return ExpenseTypes.Where(e => (e.name.ToLower().Contains(filter.ToLower())) && e.isActive == true);
    }

    public void UpdateExpenseType(int expenseTypeId, string name, float returns)
    {
        ExpenseTypes expenseType = ExpenseTypes.Find(expenseTypeId);
        expenseType.name = name;
        expenseType.returns = returns;
        SaveChanges();
    }

    public void AddExpenseType(string name, float returns)
    {
        ExpenseTypes expenseType = new ExpenseTypes();
        expenseType.name = name;
        expenseType.returns = returns;
        expenseType.isActive = true;
        ExpenseTypes.Add(expenseType);
        SaveChanges();
    }

    public void DeleteExpenseType(int expenseTypeId)
    {
        ExpenseTypes expenseType = ExpenseTypes.Find(expenseTypeId);
        expenseType.isActive = false;
        SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExpenseTypes>(cv => { cv.HasKey(["expenseTypeId"]); });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlServer(connectionString);
}