using ProjektP4.Views;

namespace ProjektP4.ViewModels;

public abstract class PageViewModelBase : ViewModelBase
{
    public MainWindowViewModel ParentViewModel { get; set; }
}