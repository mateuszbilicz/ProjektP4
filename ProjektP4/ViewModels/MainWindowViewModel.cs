using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DynamicData;
using ReactiveUI;
using Avalonia;
using Avalonia.Input;

namespace ProjektP4.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private MenuOption[] MenuOptions =
    {
        new MenuOption("Delegations", new DelegationsListPageViewModel()),
        new MenuOption("Emplyees", new EmployeesListPageViewModel()),
    };

    public ObservableCollection<MenuOption> _menuOptions {get; set;}

    public MainWindowViewModel()
    {
        _menuOptions = new ObservableCollection<MenuOption>(new List<MenuOption>(MenuOptions));
        foreach (var menuOption in _menuOptions)
        {
            menuOption.ParentViewModel = this;
        }
        _CurrentPage = MenuOptions[0].Page;
    }

    private PageViewModelBase _CurrentPage;
    
    public PageViewModelBase CurrentPage
    {
        get { return _CurrentPage; }
        private set { this.RaiseAndSetIfChanged(ref _CurrentPage, value);
            /*var loc = new ViewLocator();
            loc.Build(_CurrentPage);*/
        }
    }

    public void SetPage(PageViewModelBase page)
    {
        CurrentPage = page;
    }
}