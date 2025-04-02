﻿using System.Threading.Tasks;
using spike.Interfaces;
using spike.ViewModels;

namespace spike.Services;

public class DialogService
{
    public async Task ShowDialog<THost, TDialogViewModel>(THost host, TDialogViewModel dialogViewModel)
    where TDialogViewModel : DialogViewModel
    where THost : IDialogProvider
    {
        host.Dialog = dialogViewModel;
        dialogViewModel.Show();
        await dialogViewModel.WaitAsync();
    }
}