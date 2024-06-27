using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ProjektP4.ViewModels;

public class DelegationsListPageViewModel : PageViewModelBase
{
    public DelegationsContext db = new DelegationsContext();
    public ObservableCollection<DelegationEditable> _delegations { get; set; }
    public TextBox _purpose = new TextBox();
    public TextBox _departuePlace = new TextBox();
    public TextBox _destinationPlace = new TextBox();
    public DatePicker _startDate = new DatePicker();
    public DatePicker _endDate = new DatePicker();
    public TextBox _maxExpensePerEmployee = new TextBox();

    public DelegationsListPageViewModel()
    {
        _delegations = new ObservableCollection<DelegationEditable>(new List<DelegationEditable>());
        LoadData();
        _maxExpensePerEmployee.Text = "0";
    }

    public void LoadData()
    {
        _delegations =
            new ObservableCollection<DelegationEditable>(db.Delegations.Select(d => new DelegationEditable(d)));
    }

    private void AddDelegation_Click(object? sender, RoutedEventArgs e)
    {
        db.AddDelegation(_purpose.Text, _startDate.SelectedDate.Value.DateTime, _endDate.SelectedDate.Value.DateTime,
            _departuePlace.Text, _destinationPlace.Text, Convert.ToDecimal(_maxExpensePerEmployee.Text));
        _purpose.Text = "";
        _departuePlace.Text = "";
        _destinationPlace.Text = "";
        _startDate.SelectedDate = DateTime.Now;
        _endDate.SelectedDate = DateTime.Now;
        _maxExpensePerEmployee.Text = "0";
        LoadData();
    }

    private void DeleteDelegation_Click(object? sender, RoutedEventArgs e)
    {
        db.DeleteDelegation((int)((Button)sender).Tag);
        LoadData();
    }
}