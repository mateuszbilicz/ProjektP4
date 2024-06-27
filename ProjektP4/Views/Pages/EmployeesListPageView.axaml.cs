using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ProjektP4.ViewModels;
using ReactiveUI;

namespace ProjektP4.Views;

public partial class EmployeesListPageView : UserControl
{
    public EmployeesContext db = new EmployeesContext();
    public ObservableCollection<EmployeeEditable> _employees { get; set; }
    public string _searchText = "";
    private bool _isSubmitEnabled = false;

    public bool IsSubmitEnabled
    {
        get => _isSubmitEnabled;
        set => _isSubmitEnabled = value;
    }

    public EmployeesListPageView()
    {
        _employees = new ObservableCollection<EmployeeEditable>(new List<EmployeeEditable>());
        InitializeComponent();
        this.DataContext = new EmployeesListPageViewModel();
        LoadData();
        this.WhenAnyValue(x => x._employeeToAddFirstName.Text, x => x._employeeToAddLastName.Text,
            x => x._employeeToAddPosition.Text).Subscribe(x =>
            IsSubmitEnabled = !string.IsNullOrEmpty(x.Item1) && !string.IsNullOrEmpty(x.Item2) &&
                              !string.IsNullOrEmpty(x.Item3));
    }

    public void LoadData()
    {
        _employees.Clear();
        EmployeesListItemControl.Items.Clear();
        foreach (var employee in db.GetFiltered(_searchText))
        {
            EmployeesListItemControl.Items.Add(new EmployeeEditable(employee));
        }
    }

    private void DeleteEmployee_Click(object? sender, RoutedEventArgs e)
    {
        db.DeleteEmployee((int)((Button)sender).Tag);
        LoadData();
    }

    private void AddEmployee_Click(object? sender, RoutedEventArgs e)
    {
        if (!IsSubmitEnabled) return;
        db.AddEmployee(_employeeToAddFirstName.Text, _employeeToAddLastName.Text, _employeeToAddPosition.Text);
        _employeeToAddFirstName.Text = "";
        _employeeToAddLastName.Text = "";
        _employeeToAddPosition.Text = "";
        LoadData();
    }
}