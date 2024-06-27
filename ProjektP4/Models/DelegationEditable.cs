using System;
using System.ComponentModel;

namespace ProjektP4;

public class DelegationEditable : INotifyPropertyChanged
{
    public int DelegationId { get; }
    public string Purpose { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string DepartuePlace { get; set; }
    public string DestinationPlace { get; set; }
    public decimal MaxExpensePerEmployee { get; set; }

    public DelegationEditable(Delegations delegation)
    {
        DelegationId = delegation.delegationId;
        Purpose = delegation.purpose;
        StartDate = delegation.startDate;
        EndDate = delegation.endDate;
        DepartuePlace = delegation.departuePlace;
        DestinationPlace = delegation.destinationPlace;
        MaxExpensePerEmployee = delegation.maxExpensePerEmployee;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Purpose"));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}