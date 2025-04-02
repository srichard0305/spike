using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace spike.ViewModels;

public partial class ConfirmDialogViewModel : DialogViewModel
{
    
    [ObservableProperty]
    private string? _message = "Are you sure you want to continue?";
    [ObservableProperty]
    private string? _title = "Confirm";
    [ObservableProperty]
    private string? _confirmText = "Yes";
    [ObservableProperty]
    private string? _cancelText = "Cancel";
    
    [ObservableProperty]
    private double _dialogWidth = double.NaN;
    
    [ObservableProperty] private bool _confirmed;
    
    [RelayCommand]
    public void Confirm()
    {
        Confirmed = true;
        Close();
    }

    [RelayCommand]
    public void Cancel()
    {
        Confirmed = false;
        Close();
    }
}