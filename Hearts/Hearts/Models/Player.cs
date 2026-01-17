using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Hearts.Models
{
     public class Player
    {
        public string Name { get; set; } = "";
        public ObservableCollection<int> CumulativeScores { get; } = [];
    }
}
