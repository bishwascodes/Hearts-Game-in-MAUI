using Hearts.ViewModels;

namespace Hearts.Pages;

public partial class PlayerEntryPage : ContentPage
{
    public PlayerEntryPage(PlayerEntryViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
