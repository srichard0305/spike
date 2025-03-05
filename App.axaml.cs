using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using spike.Data;
using spike.Factories;
using spike.ViewModels;
using spike.Views;
using spike.Database;

namespace spike;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        
        //create db connection
        DatabaseConnection.InitializeDatabase();
        
        // dependency injection 
        var collection = new ServiceCollection();
        collection.AddSingleton<MainWindowViewModel>();
        collection.AddTransient<HomePageViewModel>();
        collection.AddTransient<ClientPetViewModel>();
        collection.AddTransient<EmployeePageViewModel>();
        collection.AddTransient<ReportsPageViewModel>();
        collection.AddTransient<BookAppointmentViewModel>();
        
        // used to get pageviewmodel name 
        // will only be called pages are called and brought into memory dynamically 
        // deletes page when user navigates away
        collection.AddSingleton<Func<AppPageNames, PageViewModel>>(x => name => name switch
        {
            AppPageNames.Home => x.GetRequiredService<HomePageViewModel>(),
            AppPageNames.ClientPet => x.GetRequiredService<ClientPetViewModel>(),
            AppPageNames.Employees=> x.GetRequiredService<EmployeePageViewModel>(),
            AppPageNames.Reports => x.GetRequiredService<ReportsPageViewModel>(),
            AppPageNames.BookAppointment => x.GetRequiredService<BookAppointmentViewModel>(),
            _ => throw new InvalidOperationException(),
        });
        
        collection.AddSingleton<PageFactory>();
        
        var serviceProvider = collection.BuildServiceProvider();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindowView
            {
                DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}