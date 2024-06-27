using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ProjektP4.ViewModels;
using ReactiveUI;

namespace ProjektP4.Views;

public partial class DelegationsListPageView : UserControl
{
    public DelegationsContext db = new DelegationsContext();
    public int delegationId;
    public ObservableCollection<DelegationEditable> _delegations { get; set; }
    private bool _isSubmitEnabled = false;

    public bool IsSubmitEnabled
    {
        get => _isSubmitEnabled;
        set => _isSubmitEnabled = value;
    }

    public DelegationsListPageView()
    {
        _delegations = new ObservableCollection<DelegationEditable>(new List<DelegationEditable>());
        InitializeComponent();
        this.DataContext = new DelegationsListPageViewModel();
        LoadData();
        _maxExpensePerEmployee.Text = "0";
        this.WhenAnyValue(x => x._purpose.Text, x => x._departuePlace.Text, x => x._destinationPlace.Text,
            x => x._maxExpensePerEmployee.Text).Subscribe(x =>
            IsSubmitEnabled = !string.IsNullOrEmpty(x.Item1) && !string.IsNullOrEmpty(x.Item2) &&
                              !string.IsNullOrEmpty(x.Item3) && x.Item4 != "0");
    }

    public void LoadData()
    {
        _delegations.Clear();
        DelegationsListItemControl.Items.Clear();
        foreach (var delegation in db.Delegations)
        {
            DelegationsListItemControl.Items.Add(new DelegationEditable(delegation));
        }
    }

    private void DeleteDelegation_Click(object? sender, RoutedEventArgs e)
    {
        db.DeleteDelegation((int)((Button)sender).Tag);
        LoadData();
    }

    private void AddDelegation_Click(object? sender, RoutedEventArgs e)
    {
        if (!IsSubmitEnabled || _startDate.SelectedDate == null || _endDate.SelectedDate == null ||
            _startDate.SelectedDate.Value.DateTime == DateTime.MinValue ||
            _endDate.SelectedDate.Value.DateTime == DateTime.MinValue ||
            _startDate.SelectedDate.Value.DateTime > _endDate.SelectedDate.Value.DateTime)
        {
            return;
        }

        db.AddDelegation(_purpose.Text, _startDate.SelectedDate.Value.DateTime, _endDate.SelectedDate.Value.DateTime,
            _departuePlace.Text, _destinationPlace.Text, Convert.ToDecimal(_maxExpensePerEmployee.Text));
        _purpose.Text = "";
        _departuePlace.Text = "";
        _destinationPlace.Text = "";
        _startDate.SelectedDate = null;
        _endDate.SelectedDate = null;
        _maxExpensePerEmployee.Text = "0";
        LoadData();
    }
}