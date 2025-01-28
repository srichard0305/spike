using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using spike.ViewModels;

namespace spike;

public class ViewLocator : IDataTemplate
{   
    // bind view to view model
    // ? means nullable type
    public Control? Build(object? param)
    {
        if (param is null)
            return null;
        
        // check if null with !
        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}