using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ProjektP4;

public class Delegations
{
    public int delegationId { get; set; }
    public DateTime startDate { get; set; }
    public DateTime endDate { get; set; }
    public string departuePlace { get; set; }
    public string destinationPlace { get; set; }
    public string purpose { get; set; }
    public decimal maxExpensePerEmployee { get; set; }
}

public class DelegationsContext : DbContext
{
    String connectionString = ConfigurationManager.ConnectionStrings["CompanyDelegations"].ConnectionString;
    public DbSet<Delegations> Delegations { get; set; }

    public IEnumerable<Delegations> GetFiltered(string filter)
    {
        return Delegations.Where(e =>
            (e.purpose.ToLower().Contains(filter.ToLower()) || e.departuePlace.ToLower().Contains(filter.ToLower()) ||
             e.destinationPlace.ToLower().Contains(filter.ToLower())));
    }

    public void UpdateDelegation(int delegationId, string purpose, DateTime startDate, DateTime endDate,
        string departuePlace, string destinationPlace, decimal maxExpensePerEmployee)
    {
        Delegations delegation = Delegations.Find(delegationId);
        delegation.purpose = purpose;
        delegation.startDate = startDate;
        delegation.endDate = endDate;
        delegation.departuePlace = departuePlace;
        delegation.destinationPlace = destinationPlace;
        delegation.maxExpensePerEmployee = maxExpensePerEmployee;
        SaveChanges();
    }

    public void AddDelegation(string purpose, DateTime startDate, DateTime endDate, string departuePlace,
        string destinationPlace, decimal maxExpensePerEmployee)
    {
        Delegations delegation = new Delegations();
        delegation.purpose = purpose;
        delegation.startDate = startDate;
        delegation.endDate = endDate;
        delegation.departuePlace = departuePlace;
        delegation.destinationPlace = destinationPlace;
        delegation.maxExpensePerEmployee = maxExpensePerEmployee;
        Delegations.Add(delegation);
        SaveChanges();
    }

    public void DeleteDelegation(int delegationId)
    {
        Delegations delegation = Delegations.Find(delegationId);
        Delegations.Remove(delegation);
        SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Delegations>(cv => { cv.HasKey(["delegationId"]); });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlServer(connectionString);
}