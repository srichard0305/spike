using CommunityToolkit.Mvvm.ComponentModel;
using spike.Data;

namespace spike.ViewModels;

public partial class PageViewModel : ViewModelBase
{
    [ObservableProperty]
    private AppPageNames _pageTitle;
}