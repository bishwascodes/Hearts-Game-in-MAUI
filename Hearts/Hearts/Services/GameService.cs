using System.Collections.ObjectModel;
using Hearts.Models;

namespace Hearts.Services;

public class GameService
{
    public Player[] Players { get; } =
    {
        new Player(), new Player(), new Player(), new Player()
    };

    public int Threshold { get; set; } = 100;

    // This is the "table" source for the UI
    public ObservableCollection<RoundScore> Rounds { get; } = new();

    public bool IsGameOver { get; private set; }
    public int WinnerIndex { get; private set; } = -1;

    public void Reset()
    {
        foreach (var p in Players) p.Name = "";
        Threshold = 100;
        Rounds.Clear();
        IsGameOver = false;
        WinnerIndex = -1;
    }

    public int TotalFor(int playerIndex) =>
        playerIndex switch
        {
            0 => Rounds.Sum(r => r.P1),
            1 => Rounds.Sum(r => r.P2),
            2 => Rounds.Sum(r => r.P3),
            3 => Rounds.Sum(r => r.P4),
            _ => 0
        };

    public void AddRound(int s1, int s2, int s3, int s4)
    {
        if (IsGameOver) return;

        Rounds.Add(new RoundScore
        {
            RoundNumber = Rounds.Count + 1,
            P1 = s1,
            P2 = s2,
            P3 = s3,
            P4 = s4
        });

        // Simple win rule: first to reach threshold
        for (int i = 0; i < 4; i++)
        {
            if (TotalFor(i) >= Threshold)
            {
                IsGameOver = true;
                WinnerIndex = i;
                break;
            }
        }
    }
}
