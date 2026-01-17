using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearts.Services;
using Hearts.Pages;

namespace Hearts.ViewModels;

public partial class PlayerEntryViewModel : ObservableObject
{
    private readonly GameService _game;

    public PlayerEntryViewModel(GameService game)
    {
        _game = game;
        Threshold = _game.Threshold;
    }

    [ObservableProperty] 
    private string player1 = "";

    [ObservableProperty] 
    private string player2 = "";

    [ObservableProperty] 
    private string player3 = "";

    [ObservableProperty] 
    private string player4 = "";

    [ObservableProperty] 
    private int threshold = 100;

    [RelayCommand]
    private async Task Start()
    {
        _game.Players[0].Name = string.IsNullOrWhiteSpace(Player1) ? "Player 1" : Player1.Trim();
        _game.Players[1].Name = string.IsNullOrWhiteSpace(Player2) ? "Player 2" : Player2.Trim();
        _game.Players[2].Name = string.IsNullOrWhiteSpace(Player3) ? "Player 3" : Player3.Trim();
        _game.Players[3].Name = string.IsNullOrWhiteSpace(Player4) ? "Player 4" : Player4.Trim();

        _game.Threshold = Threshold <= 0 ? 100 : Threshold;

        await Shell.Current.GoToAsync(nameof(ScorePage));
    }
}

