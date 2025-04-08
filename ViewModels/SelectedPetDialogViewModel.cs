using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using spike.Models;

namespace spike.ViewModels;

public partial class SelectedPetDialogViewModel : ConfirmDialogViewModel
{
    [ObservableProperty]
    private ObservableCollection<Pet> _pets;
    
    [ObservableProperty]
    private Pet _selectedPet;
    
    public SelectedPetDialogViewModel()
    {
    }

    public SelectedPetDialogViewModel(ObservableCollection<Pet> pets)
    {
        Pets = pets;
    }

    [RelayCommand]
    private void SelectPet(Pet pet)
    {
        SelectedPet = pet;
        Confirm();
    }

    
    
    
}