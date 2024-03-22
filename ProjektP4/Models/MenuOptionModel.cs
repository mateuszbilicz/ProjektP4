using System;
using System.ComponentModel;
using System.Reactive;
using Avalonia.Controls.Mixins;
using Avalonia.MicroCom;
using ProjektP4.ViewModels;

namespace ProjektP4;

public class MenuOption : INotifyPropertyChanged
{
    public string Label { get; set; }
    public PageViewModelBase Page { get; set; }
    public ViewModelBase ParentViewModel { get; set; }
    
    public MenuOption(string label, PageViewModelBase page)
    {
        Label = label;
        Page = page;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Label"));
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
}