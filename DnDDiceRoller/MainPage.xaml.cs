using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDDiceRoller
{
    public partial class MainPage : ContentPage
    {
        private Dictionary<int, int> selectedDice = new Dictionary<int, int>();
        private SignalRService _signalRService;
        private string _username;
        private List<RollHistoryItem> rollHistory = new List<RollHistoryItem>();

        public MainPage()
        {
            InitializeComponent();
            _signalRService = new SignalRService();
            _signalRService.RollReceived += SignalRService_RollReceived;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Task.Run(async () => await _signalRService.DisconnectAsync());
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            _username = UsernameEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(_username))
            {
                await DisplayAlert("Error", "Please enter a valid username.", "OK");
                return;
            }

            LoginSection.IsVisible = false;
            MainAppSection.IsVisible = true;

            try
            {
                await _signalRService.InitializeAsync("https://dnddicerollerapi20240821154836.azurewebsites.net/rollHub");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not connect to server: {ex.Message}", "OK");
            }
        }

        private void AddDiceButton_Clicked(object sender, EventArgs e)
        {
            string selectedDiceType = DiceTypePicker.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(selectedDiceType) || string.IsNullOrWhiteSpace(DiceQuantityEntry.Text))
            {
                DisplayAlert("Error", "Please select a dice type and enter a quantity.", "OK");
                return;
            }

            if (!int.TryParse(DiceQuantityEntry.Text, out int quantity) || quantity <= 0)
            {
                DisplayAlert("Error", "Please enter a valid quantity.", "OK");
                return;
            }

            int sides = int.Parse(selectedDiceType.Substring(1)); // Extract sides from the dice type string

            if (selectedDice.ContainsKey(sides))
            {
                selectedDice[sides] += quantity;
            }
            else
            {
                selectedDice[sides] = quantity;
            }

            UpdateSelectedDiceList();
            DiceQuantityEntry.Text = string.Empty;
        }

        private void UpdateSelectedDiceList()
        {
            var diceList = selectedDice.Select(d => $"{d.Value}d{d.Key}").ToList();
            SelectedDiceCollectionView.ItemsSource = diceList;
        }

        private async void RollButton_Clicked(object sender, EventArgs e)
        {
            if (!selectedDice.Any())
            {
                await DisplayAlert("Error", "Please add some dice before rolling.", "OK");
                return;
            }

            int modifier = 0;
            if (!string.IsNullOrWhiteSpace(ModifierEntry.Text))
            {
                if (!int.TryParse(ModifierEntry.Text, out modifier))
                {
                    await DisplayAlert("Error", "Please enter a valid modifier.", "OK");
                    return;
                }
            }

            DiceRoller roller = new DiceRoller();
            List<int> individualRolls = roller.RollMultipleDice(selectedDice);
            int total = individualRolls.Sum() + modifier;

            // Update local roll history
            var rollDetails = $"{string.Join(", ", selectedDice.Select(d => $"{d.Value}d{d.Key}"))}";
            if (modifier != 0)
                rollDetails += $" + {modifier}";

            var rollItem = new RollHistoryItem
            {
                User = _username,
                Total = total,
                IndividualRolls = individualRolls,
                RollDetails = $"{rollDetails} = {total}",
                Timestamp = DateTime.Now.ToString("g"),
                DiceUsed = selectedDice // Store the dice used
            };

            rollHistory.Insert(0, rollItem);
            RollHistoryCollectionView.ItemsSource = null;
            RollHistoryCollectionView.ItemsSource = rollHistory;

            // Send roll and dice information to other users via SignalR
            await _signalRService.SendRoll(_username, total, individualRolls, selectedDice);
        }

        private void ClearDiceButton_Clicked(object sender, EventArgs e)
        {
            selectedDice.Clear();
            UpdateSelectedDiceList();
            ModifierEntry.Text = string.Empty;
        }

        private void SignalRService_RollReceived(object sender, RollEventArgs e)
{
    if (e.User == _username)
        return; // Ignore own rolls

    var rollDetails = $"Rolls: {string.Join(", ", e.IndividualRolls)} = {e.Total}";
    var diceDetails = $"Dice Used: {string.Join(", ", e.DiceUsed.Select(d => $"{d.Value}d{d.Key}"))}";

    var rollItem = new RollHistoryItem
    {
        User = e.User,
        Total = e.Total,
        IndividualRolls = e.IndividualRolls,
        RollDetails = rollDetails,
        Timestamp = DateTime.Now.ToString("g"),
        DiceUsed = e.DiceUsed  // Assign the DiceUsed property
    };

    Device.BeginInvokeOnMainThread(() =>
    {
        rollHistory.Insert(0, rollItem);
        RollHistoryCollectionView.ItemsSource = null;
        RollHistoryCollectionView.ItemsSource = rollHistory;
        DisplayAlert("New Roll", $"{e.User} rolled:\n{rollDetails}\n{diceDetails}", "OK");
    });
}
        public class RollHistoryItem
        {
            public string User { get; set; }
            public int Total { get; set; }
            public List<int> IndividualRolls { get; set; }
            public string RollDetails { get; set; }
            public string Timestamp { get; set; }
            public Dictionary<int, int> DiceUsed { get; internal set; }
        }
    }
}
