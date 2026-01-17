using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearts.Models;
using Hearts.Services;
using System.Collections.ObjectModel;

namespace Hearts.ViewModels;

public partial class ScoreViewModel : ObservableObject
{
    private readonly GameService _game;

    public ScoreViewModel(GameService game)
    {
        _game = game;

        // When rounds change, totals should refresh
        _game.Players.CollectionChanged += (_, __) =>
        {
            OnPropertyChanged(nameof(T1));
            OnPropertyChanged(nameof(T2));
            OnPropertyChanged(nameof(T3));
            OnPropertyChanged(nameof(T4));
        };

        UpdateStatus();
    }

    // Names shown on screen (come from GameService)
    public string P1Name => _game.Players[0].Name;
    public string P2Name => _game.Players[1].Name;
    public string P3Name => _game.Players[2].Name;
    public string P4Name => _game.Players[3].Name;

    // Expose threshold so UI pieces (graph) can use it
    public int Threshold => _game.Threshold;

    // CollectionView uses this as its ItemsSource
    public ObservableCollection<Player> Rounds => _game.Players;

    // Round inputs
    [ObservableProperty] 
    private int s1;

    [ObservableProperty] 
    private int s2;

    [ObservableProperty] 
    private int s3;

    [ObservableProperty] 
    private int s4;

    // Totals (computed)
    public int T1 => _game.TotalFor(0);
    public int T2 => _game.TotalFor(1);
    public int T3 => _game.TotalFor(2);
    public int T4 => _game.TotalFor(3);

    [ObservableProperty] 
    private string statusText = "";

    [ObservableProperty] 
    private bool isGameOver;

    [RelayCommand(CanExecute = nameof(CanSubmit))]
    private void SubmitRound()
    {
        _game.AddRound(S1, S2, S3, S4);

        // clear for next round
        S1 = S2 = S3 = S4 = 0;

        // Update status and check if game is over
        UpdateStatus();
        
        SubmitRoundCommand.NotifyCanExecuteChanged();
    }

    private bool CanSubmit() => !_game.IsGameOver;

    [RelayCommand]
    private async Task PlayAgain()
    {
        _game.Reset();
        
        // Clear input fields
        S1 = S2 = S3 = S4 = 0;
        
        // Notify all properties to refresh
        OnPropertyChanged(nameof(T1));
        OnPropertyChanged(nameof(T2));
        OnPropertyChanged(nameof(T3));
        OnPropertyChanged(nameof(T4));
        OnPropertyChanged(nameof(Rounds));
        OnPropertyChanged(nameof(Threshold));
        
        UpdateStatus();
        SubmitRoundCommand.NotifyCanExecuteChanged();
        
        // Navigate back to player entry page
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task ShowRules()
    {
        await Shell.Current.GoToAsync(nameof(Pages.RulesPage));
    }

    private void UpdateStatus()
    {
        IsGameOver = _game.IsGameOver;

        if (_game.IsGameOver)
        {
            StatusText = $"Game Over — Winner: {_game.Players[_game.WinnerIndex].Name}";
        }
        else
        {
            StatusText = $"Round input: first to reach {_game.Threshold} wins.";
        }
    }
}
