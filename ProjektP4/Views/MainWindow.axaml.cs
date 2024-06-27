using Avalonia.Controls;
using Avalonia.Interactivity;
using ProjektP4.ViewModels;

namespace ProjektP4.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void InputElement_OnPointerPressed(object? sender, RoutedEventArgs routedEventArgs)
    {
        if (sender.Equals(null)) return;
        var tb = sender as Button;
        var menuOp = tb.DataContext as MenuOption;
        var parent = menuOp.ParentViewModel as MainWindowViewModel;
        menuOp.Page.ParentViewModel = parent;
        parent.SetPage(menuOp.Page);
    }
}