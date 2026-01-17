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
@"
Hearts is a four player card game. The goal of the game is to score as few points as possible.
To start the game, deal a traditional 52 card deck of playing cards equally among four players.

Game Play:
- The player holding the 2 of clubs starts the first round by playing that card.
- Play then proceeds clockwise, with each player playing one card per round.
- Players must follow suit if they can; if they cannot, they may play any card.
- The round is won by the player who played the highest card of the suit that was 'led' (the first card played).
- The winner of the round collects all the cards played and leads the next round.
- At the end of each hand (after all cards have been played), players tally their points based on the cards they have collected.

Special Rules on the first round:
- No player may play a point card (any heart or the queen of spades) on the first trick, unless they have no alternative.

Leading Hearts and the Queen of Spades:
- Hearts cannot be led until a heart has been played in a previous round (this is known as 'breaking hearts').
- The queen of spades can be played at any time, regardless of whether hearts have been broken.

'Shooting the Moon':
- If a player manages to collect all the hearts and the queen of spades in a single hand, they have 'shot the moon'.
- Instead of receiving 26 points, that player may choose to have all other players receive 26 points, or they may subtract 26 points from their own score.

Scoring:
- Each heart card is worth 1 point.
- The queen of spades is worth 13 points.

- The game continues until one or more players reach or exceed 100 points (or whatever threshold the players agree on). The player with the lowest score at that time is declared the winner.
";

    [RelayCommand]
    private async Task Begin()
    {
        _game.Reset();
        await Shell.Current.GoToAsync(nameof(PlayerEntryPage));
    }
}
