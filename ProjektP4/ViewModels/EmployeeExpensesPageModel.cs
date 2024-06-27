using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ProjektP4.ViewModels;

public class EmployeeExpensesPageViewModel : PageViewModelBase
{
    public EmployeeExpensesContext db = new EmployeeExpensesContext();
    public CurrencyValuesContext currencyDb = new CurrencyValuesContext();
    public ExpenseTypesContext expTypesDb = new ExpenseTypesContext();
    public DelegationsContext delegationsDb = new DelegationsContext();
    public int delegationId;
    
    public ObservableCollection<EmployeeExpenses> _expenses { get; set; }
    public ComboBox _delegationSelect = new ComboBox();
    public ComboBox _employeeSelect = new ComboBox();
    public ComboBox _currencySelect = new ComboBox();
    public ComboBox _expenseTypeSelect = new ComboBox();
    public TextBox _cost = new TextBox();
    
    public TextBox DelegationPurpose = new TextBox();
    public TextBox DelegationStartDate = new TextBox();
    public TextBox DelegationEndDate = new TextBox();
    
    public EmployeeExpensesPageViewModel()
    {
        _expenses = new ObservableCollection<EmployeeExpenses>(new List<EmployeeExpenses>());
        LoadData();
        db.CurrencyValues.Select(x => x.name).ToList().ForEach(x => _currencySelect.Items.Add(x));
        db.ExpenseTypes.Where(x => x.isActive).Select(x => x.name).ToList()
            .ForEach(x => _expenseTypeSelect.Items.Add(x));
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
        
        _delegationSelect.Items.Clear();
        foreach (var delegation in delegationsDb.Delegations)
        {
            _delegationSelect.Items.Add(delegation.purpose);
        }
    }

    public void LoadExpenses()
    {
        delegationId = db.Delegations.Where(x => x.purpose == _delegationSelect.SelectedItem.ToString()).Select(x => x.delegationId).FirstOrDefault();
        _expenses = new ObservableCollection<EmployeeExpenses>(db.GetFiltered(delegationId).ToList());
    }

    private void AddExpense_Click(object? sender, RoutedEventArgs e)
    {
        if ( _cost.Text == "0" || _cost.Text == "" || _cost.Text == null ||
             _delegationSelect.SelectedItem == null ||
             _employeeSelect.SelectedItem == null ||
            _currencySelect.SelectedItem == null || _expenseTypeSelect.SelectedItem == null)
        {
            return;
        }

        var selectedDelegation = _delegationSelect.SelectedItem.ToString();
        var selectedEmployee = _employeeSelect.SelectedItem.ToString();
        var selectedCurrency = _currencySelect.SelectedItem.ToString();
        var selectedType = _expenseTypeSelect.SelectedItem.ToString();
        db.AddExpense(
            db.Delegations.Where(x => x.purpose == selectedDelegation).Select(x => x.delegationId).FirstOrDefault(),
            db.Employees.Where(x => (x.firstName + " " + x.lastName) == selectedEmployee).Select(x => x.employeeId).FirstOrDefault(),
            Convert.ToDecimal(_cost.Text),
            db.ExpenseTypes.Where(x => x.name == selectedType).Select(x => x.expenseTypeId).FirstOrDefault(),
            db.CurrencyValues.Where(x => x.name == selectedCurrency).Select(x => x.currencyValueId).FirstOrDefault()
            );
        _cost.Text = "0";
        _currencySelect.SelectedItem = null;
        _expenseTypeSelect.SelectedItem = null;
        LoadData();
    }

    private void DeleteExpense_Click(object? sender, RoutedEventArgs e)
    {
        db.DeleteExpense((int)((Button)sender).Tag);
        LoadData();
    }
}