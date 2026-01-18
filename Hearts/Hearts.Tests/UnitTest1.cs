namespace Hearts.Tests
{
    using Hearts.Services;
    using Xunit;

    public class PlayerEntryViewModelTests
    {
        private readonly GameService _gameService = new();

        [Fact]
        public void Constructor_InitializesPlayerNames()
        {
            var vm = new Hearts.ViewModels.PlayerEntryViewModel(_gameService);
            
            Assert.Equal("", vm.Player1);
            Assert.Equal("", vm.Player2);
            Assert.Equal("", vm.Player3);
            Assert.Equal("", vm.Player4);
        }

        [Fact]
        public void Constructor_InitializesThresholdFromGameService()
        {
            _gameService.Threshold = 150;
            var vm = new Hearts.ViewModels.PlayerEntryViewModel(_gameService);
            
            Assert.Equal(150, vm.Threshold);
        }

        [Fact]
        public void Constructor_UsesDefaultThreshold()
        {
            var vm = new Hearts.ViewModels.PlayerEntryViewModel(_gameService);
            
            Assert.Equal(100, vm.Threshold);
        }

        [Theory]
        [InlineData(nameof(Hearts.ViewModels.PlayerEntryViewModel.Player1), "Alice")]
        [InlineData(nameof(Hearts.ViewModels.PlayerEntryViewModel.Player2), "Bob")]
        [InlineData(nameof(Hearts.ViewModels.PlayerEntryViewModel.Player3), "Charlie")]
        [InlineData(nameof(Hearts.ViewModels.PlayerEntryViewModel.Player4), "Diana")]
        public void PlayerNames_RaisePropertyChanged(string propertyName, string value)
        {
            var vm = new Hearts.ViewModels.PlayerEntryViewModel(_gameService);
            var changed = false;

            vm.PropertyChanged += (s, e) => 
            {
                if (e.PropertyName == propertyName)
                    changed = true;
            };

            var property = vm.GetType().GetProperty(propertyName);
            property?.SetValue(vm, value);

            Assert.True(changed);
        }

        [Fact]
        public void Threshold_RaisesPropertyChanged()
        {
            var vm = new Hearts.ViewModels.PlayerEntryViewModel(_gameService);
            var changed = false;

            vm.PropertyChanged += (s, e) => 
            {
                if (e.PropertyName == nameof(Hearts.ViewModels.PlayerEntryViewModel.Threshold))
                    changed = true;
            };

            vm.Threshold = 250;

            Assert.True(changed);
            Assert.Equal(250, vm.Threshold);
        }

        [Fact]
        public void AllPlayerNames_CanBeSet()
        {
            var vm = new Hearts.ViewModels.PlayerEntryViewModel(_gameService);

            vm.Player1 = "Alice";
            vm.Player2 = "Bob";
            vm.Player3 = "Charlie";
            vm.Player4 = "Diana";

            Assert.Equal("Alice", vm.Player1);
            Assert.Equal("Bob", vm.Player2);
            Assert.Equal("Charlie", vm.Player3);
            Assert.Equal("Diana", vm.Player4);
        }

        [Fact]
        public void Threshold_CanBeSet()
        {
            var vm = new Hearts.ViewModels.PlayerEntryViewModel(_gameService);
            vm.Threshold = 500;
            
            Assert.Equal(500, vm.Threshold);
        }

        [Fact]
        public void MultipleInstances_HaveIndependentThreshold()
        {
            _gameService.Threshold = 200;
            var vm1 = new Hearts.ViewModels.PlayerEntryViewModel(_gameService);
            var vm2 = new Hearts.ViewModels.PlayerEntryViewModel(_gameService);

            vm1.Threshold = 300;

            Assert.Equal(300, vm1.Threshold);
            Assert.Equal(200, vm2.Threshold);
        }
    }
}
