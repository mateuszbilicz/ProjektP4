using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ProjektP4.Views;

public partial class EmployeesListPageView : UserControl
{
    
    // TODO: Something is wrong with this page
    public EmployeesContext db = new EmployeesContext();

    public ObservableCollection<EmployeeEditable> _employees {get; set;}

    public string _searchText = "";
    
    public EmployeesListPageView()
    {
        InitializeComponent();
        LoadData();
    }

    public void LoadData()
    {
        _employees = new ObservableCollection<EmployeeEditable>(new List<EmployeeEditable>());
        
        foreach (var employee in db.GetFiltered(_searchText))
        {
            _employees.Add(new EmployeeEditable(employee));
        }
    }

    private void DeleteEmployee_Click(object? sender, RoutedEventArgs e)
    {
        db.DeleteEmployee((int)((Button)sender).Tag);
    }

    private void AddEmployee_Click(object? sender, RoutedEventArgs e)
    {
        db.AddEmployee(
            _employeeToAddFirstName.Text,
            _employeeToAddLastName.Text,
            _employeeToAddPosition.Text
        );
        _employeeToAddFirstName.Text = "";
        _employeeToAddLastName.Text = "";
        _employeeToAddPosition.Text = "";
        LoadData();
    }
}