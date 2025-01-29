using System;
using spike.Data;
using spike.ViewModels;

namespace spike.Factories;

// factory for creating pages dynamically when page is navigated too 
public class PageFactory(Func<AppPageNames, PageViewModel> pageViewModelFactory)
{
    public PageViewModel GetPageViewModel(AppPageNames pageName) => pageViewModelFactory.Invoke(pageName);
}