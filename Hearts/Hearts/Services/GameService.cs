using System.Collections.ObjectModel;
using System.Linq;
using Hearts.Models;

namespace Hearts.Services;

public class GameService
{
    private static readonly Player Player1 = new();
    private static readonly Player Player2 = new();
    private static readonly Player Player3 = new();
    private static readonly Player Player4 = new();

    public GameService()
    {
        Players = new ObservableCollection<Player>
        {
            Player1,
            Player2,
            Player3,
            Player4
        };
    }

    public ObservableCollection<Player> Players { get; }

    public int Threshold { get; set; } = 100;

    public bool IsGameOver { get; private set; }
    public int WinnerIndex { get; private set; } = -1;

    public void Reset()
    {
        foreach (var p in Players)
        {
            p.Name = string.Empty;
            p.CumulativeScores.Clear();
        }
        Threshold = 100;
        IsGameOver = false;
        WinnerIndex = -1;
    }

    public int TotalFor(int playerIndex)
        {
            return Players[playerIndex].CumulativeScores.Sum();
        }

    public void AddRound(int s1, int s2, int s3, int s4)
    {
        if (IsGameOver) return;

        Players[0].CumulativeScores.Add(s1);
        Players[1].CumulativeScores.Add(s2);
        Players[2].CumulativeScores.Add(s3);
        Players[3].CumulativeScores.Add(s4);

        // Simple win rule: first to reach threshold
        for (int i = 0; i < 4; i++)
        {
            if (TotalFor(i) >= Threshold)
            {
                IsGameOver = true;
                // WinnerIndex should be the player with the lowest total
                WinnerIndex = Players.Select((p, idx) => new { Total = p.CumulativeScores.Sum(), idx })
                                      .OrderBy(x => x.Total)
                                      .First().idx;
                break;
            }
        }
    }
}
