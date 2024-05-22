using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ProjektP4;

public class Employees
{
    public int employeeId { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string position { get; set; }
    public bool isActive { get; set; }
}

public class EmployeesContext : DbContext
{
    String connectionString = ConfigurationManager.ConnectionStrings["CompanyDelegations"].ConnectionString;

    public DbSet<Employees> Employees { get; set; }

    public IEnumerable<Employees> GetFiltered(
        string filter
    )
    {
        return Employees.Where(e =>
            (
                e.firstName.ToLower().Contains(filter.ToLower())
                || e.lastName.ToLower().Contains(filter.ToLower())
                || e.position.ToLower().Contains(filter.ToLower())
            )
            && e.isActive == true
        );
    }

    public void UpdateEmployee(
        int employeeId,
        string firstName,
        string lastName,
        string position
    )
    {
        Employees employee = Employees.Find(employeeId);
        employee.firstName = firstName;
        employee.lastName = lastName;
        employee.position = position;
        SaveChanges();
    }

    public void AddEmployee(
        string firstName,
        string lastName,
        string position
    )
    {
        Employees employee = new Employees();
        employee.firstName = firstName;
        employee.lastName = lastName;
        employee.position = position;
        employee.isActive = true;
        Employees.Add(employee);
        SaveChanges();
    }

    public void DeleteEmployee(
        int employeeId
    )
    {
        Employees employee = Employees.Find(employeeId);
        employee.isActive = false;
        SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employees>(
            cv => { cv.HasKey(["employeeId"]); }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(connectionString);
}