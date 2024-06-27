using System.Linq;

namespace ProjektP4;

public class EmployeeExpensesDisplay
{
    public int EmployeeExpenseId { get; set; }
    public int DelegationId { get; set; }
    public int EmployeeId { get; set; }
    public decimal Cost { get; set; }
    public int CurrencyId { get; set; }
    public int TypeId { get; set; }
    public string EmployeeName { get; set; }
    public string CurrencyName { get; set; }
    public string TypeName { get; set; }
    
    public EmployeeExpensesContext db = new EmployeeExpensesContext();

    public EmployeeExpensesDisplay(EmployeeExpenses expense)
    {
        EmployeeExpenseId = expense.EmployeeExpenseId;
        DelegationId = expense.DelegationId;
        EmployeeId = expense.EmployeeId;
        Cost = expense.Cost;
        CurrencyId = expense.CurrencyId;
        TypeId = expense.TypeId;
        var employee = db.Employees.Where(x => x.employeeId == EmployeeId).Select(x => x).FirstOrDefault();
        EmployeeName = employee.firstName + " " + employee.lastName;
        CurrencyName = db.CurrencyValues.Where(x => x.currencyValueId == CurrencyId).Select(x => x.name).FirstOrDefault();
        TypeName = db.ExpenseTypes.Where(x => x.expenseTypeId == TypeId).Select(x => x.name).FirstOrDefault();
    }
}