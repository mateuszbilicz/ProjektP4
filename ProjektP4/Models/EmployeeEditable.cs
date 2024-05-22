using System.ComponentModel;

namespace ProjektP4;

public class EmployeeEditable : INotifyPropertyChanged
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Position { get; set; }
    public int EmployeeId { get; }
    
    public EmployeeEditable(Employees employee)
    {
        FirstName = employee.firstName;
        LastName = employee.lastName;
        Position = employee.position;
        EmployeeId = employee.employeeId;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FirstName"));
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
}