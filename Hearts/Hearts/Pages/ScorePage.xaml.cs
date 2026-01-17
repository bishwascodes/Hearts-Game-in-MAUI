using Hearts.ViewModels;

namespace Hearts.Pages;

public partial class ScorePage : ContentPage
{
    public ScorePage(ScoreViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private void OnEntryFocused(object sender, FocusEventArgs e)
    {
        if (sender is Entry entry && entry.Text == "0")
        {
            entry.Text = string.Empty;
        }
    }
}