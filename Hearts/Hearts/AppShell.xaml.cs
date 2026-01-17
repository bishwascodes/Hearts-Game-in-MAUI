using Hearts.Pages;

namespace Hearts
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(PlayerEntryPage), typeof(PlayerEntryPage));
            Routing.RegisterRoute(nameof(ScorePage), typeof(ScorePage));
        }
    }
}
