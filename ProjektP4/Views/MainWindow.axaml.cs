using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using ProjektP4.ViewModels;
using ReactiveUI;
using Splat;

namespace ProjektP4.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private void InputElement_OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (sender.Equals(null)) return;
        var tb = sender as TextBlock;
        var menuOp = tb.DataContext as MenuOption;
        var parent = menuOp.ParentViewModel as MainWindowViewModel;
        parent.SetPage(menuOp.Page);
        // throw new System.NotImplementedException();
    }
}