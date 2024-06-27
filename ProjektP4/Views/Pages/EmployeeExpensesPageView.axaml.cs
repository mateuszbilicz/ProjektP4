using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ProjektP4.ViewModels;
using ReactiveUI;

namespace ProjektP4.Views;

public partial class EmployeeExpensesPageView : UserControl
{
    public EmployeeExpensesContext db = new EmployeeExpensesContext();
    public CurrencyValuesContext currencyDb = new CurrencyValuesContext();
    public ExpenseTypesContext expTypesDb = new ExpenseTypesContext();
    public EmployeesContext employeesDb = new EmployeesContext();
    public DelegationsContext delegationsDb = new DelegationsContext();
    public int delegationId;
    
    public ObservableCollection<EmployeeExpensesDisplay> _expenses { get; set; }

    private bool _isSubmitEnabled = false;

    public bool IsSubmitEnabled
    {
        get => _isSubmitEnabled;
        set => _isSubmitEnabled = value;
    }

    public EmployeeExpensesPageView()
    {
        InitializeComponent();
        _expenses = new ObservableCollection<EmployeeExpensesDisplay>();
        this.DataContext = new EmployeeExpensesPageViewModel();
        this.WhenAnyValue(x => x._cost.Text,
            x => x._currencySelect.SelectedItem,
            x => x._expenseTypeSelect.SelectedItem,
            x => x._employeeSelect.SelectedItem,
            x => x._delegationSelect.SelectedItem).Subscribe(x =>
            IsSubmitEnabled = !string.IsNullOrEmpty(x.Item1)
                                                             && x.Item2 != null
                                                             && x.Item3 != null
                                                             && x.Item4 != null
                                                             && x.Item5 != null);
        LoadData();
        DelegationPurpose.Text = "Please, select delegation";
    }

    public void LoadData()
    {
        _currencySelect.Items.Clear();
        foreach (var currency in currencyDb.CurrencyValues)
        {
            _currencySelect.Items.Add(currency.name);
        }
        
        _expenseTypeSelect.Items.Clear();
        foreach (var type in expTypesDb.ExpenseTypes.Where(x => x.isActive))
        {
            _expenseTypeSelect.Items.Add(type.name);
        }
        
        _employeeSelect.Items.Clear();
        foreach (var employee in employeesDb.Employees.Where(x => x.isActive))
        {
            _employeeSelect.Items.Add(employee.firstName + " " + employee.lastName);
        }
        
        _delegationSelect.Items.Clear();
        foreach (var delegation in delegationsDb.Delegations)
        {
            _delegationSelect.Items.Add(delegation.purpose);
        }
    }
    
    public void LoadExpenses()
    {
        var delegation = db.Delegations.Where(x => x.purpose == _delegationSelect.SelectedItem.ToString())
            .FirstOrDefault();
        if (delegation == null)
        {
            _delegationSelect.SelectedItem = null;
            DelegationPurpose.Text = "Please, select delegation";
            DelegationStartDate.Text = "";
            DelegationEndDate.Text = "";
            LoadData();
            return;
        }

        DelegationPurpose.Text = "Purpose: " + delegation.purpose;
        DelegationStartDate.Text = "Start date: " + delegation.startDate;
        DelegationEndDate.Text = "End date: " + delegation.endDate;
        delegationId = delegation.delegationId;
        _expenses.Clear();
        ExpensesListItemControl.Items.Clear();
        foreach (var expense in db.GetFiltered(delegationId))
        {
            ExpensesListItemControl.Items.Add(new EmployeeExpensesDisplay(expense));
        }
    }

    private void DeleteExpense_Click(object? sender, RoutedEventArgs e)
    {
        db.DeleteExpense((int)((Button)sender).Tag);
        LoadData();
    }

    private void AddExpense_Click(object? sender, RoutedEventArgs e)
    {
        if (!IsSubmitEnabled) return;
        var selectedDelegation = _delegationSelect.SelectedItem.ToString();
        var selectedEmployee = _employeeSelect.SelectedItem.ToString();
        var selectedCurrency = _currencySelect.SelectedItem.ToString();
        var selectedType = _expenseTypeSelect.SelectedItem.ToString();
        db.AddExpense(
            db.Delegations.Where(x => x.purpose == selectedDelegation).Select(x => x.delegationId).FirstOrDefault(),
            db.Employees.Where(x => (x.firstName + " " + x.lastName) == selectedEmployee).Select(x => x.employeeId).FirstOrDefault(),
            Convert.ToDecimal(_cost.Text),
            db.ExpenseTypes.Where(x => x.name == selectedType).Select(x => x.expenseTypeId).FirstOrDefault(),
            db.CurrencyValues.Where(x => x.name == selectedCurrency).Select(x => x.currencyValueId).FirstOrDefault());
        _cost.Text = "0";
        _currencySelect.SelectedItem = null;
        _expenseTypeSelect.SelectedItem = null;
        LoadData();
    }

    private void _delegationSelect_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        LoadExpenses();
    }
}