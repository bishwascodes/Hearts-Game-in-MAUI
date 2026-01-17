using Hearts.Models;
using Hearts.Services;
using Hearts.ViewModels;
using Microsoft.Maui.Graphics;
using System.Collections.Specialized;

namespace Hearts.Pages;

public partial class ScorePage : ContentPage
{
    private readonly ScoreGraph _drawable = new();
    public ScorePage(ScoreViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

        // Assign players collection from the GameService via the ViewModel
        if (vm is not null)
        {
            _drawable.Players = vm.Rounds;
            _drawable.Threshold = vm.Threshold;

            // Invalidate the graphics view when scores change
            if (_drawable.Players != null)
            {
                _drawable.Players.CollectionChanged += Players_CollectionChanged;
                foreach (var p in _drawable.Players)
                {
                    p.CumulativeScores.CollectionChanged += PlayerScores_CollectionChanged;
                }
            }

            // If threshold might change (e.g., PlayAgain), listen for property changes on VM
            if (vm is System.ComponentModel.INotifyPropertyChanged npc)
            {
                npc.PropertyChanged += (_, __) =>
                {
                    _drawable.Threshold = vm.Threshold;
                    ScoreCanvas.Invalidate();
                };
            }
        }

        ScoreCanvas.Drawable = _drawable;
    }

    private void Players_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // subscribe to new players' score changes
        if (e.NewItems != null)
        {
            foreach (Player p in e.NewItems)
                p.CumulativeScores.CollectionChanged += PlayerScores_CollectionChanged;
        }

        if (e.OldItems != null)
        {
            foreach (Player p in e.OldItems)
                p.CumulativeScores.CollectionChanged -= PlayerScores_CollectionChanged;
        }

        ScoreCanvas.Invalidate();
    }

    private void PlayerScores_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ScoreCanvas.Invalidate();
    }

    private void OnEntryFocused(object sender, FocusEventArgs e)
    {
        if (sender is Entry entry && entry.Text == "0")
        {
            entry.Text = string.Empty;
        }
    }
}
