using Hearts.ViewModels;

namespace Hearts.Pages;

public partial class RulesPage : ContentPage
{
    public RulesPage(RulesViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}