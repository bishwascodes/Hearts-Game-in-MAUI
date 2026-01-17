using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearts.Services;
using Hearts.Pages;

namespace Hearts.ViewModels;

public partial class RulesViewModel : ObservableObject
{
    private readonly GameService _game;

    public RulesViewModel(GameService game) => _game = game;

    public string RulesText =>
@"Hearts (abstract scorekeeper)
1) Enter 4 player names
2) Set threshold
3) Each round enter scores
4) First to reach threshold wins";

    [RelayCommand]
    private async Task Begin()
    {
        _game.Reset();
        await Shell.Current.GoToAsync(nameof(PlayerEntryPage));
    }
}
