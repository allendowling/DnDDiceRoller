using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDDiceRoller
{
    public partial class MainPage : ContentPage
    {
        private DiceRoller diceRoller = new DiceRoller();
        private Dictionary<int, int> selectedDice = new Dictionary<int, int>();
        private SignalRService _signalRService;
        private string _username;
        private RollHistoryManager _rollHistoryManager;

        public MainPage()
        {
            InitializeComponent();
            _signalRService = new SignalRService();
            _signalRService.RollReceived += SignalRService_RollReceived;
            _rollHistoryManager = new RollHistoryManager();

            // Bind the CollectionView to the RollHistoryManager's RollHistory collection
            RollHistoryCollectionView.ItemsSource = _rollHistoryManager.RollHistory;
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

            int modifier = int.TryParse(ModifierEntry.Text, out var mod) ? mod : 0;

            // Roll the dice
            var individualRolls = diceRoller.RollMultipleDice(selectedDice, modifier);
            int total = individualRolls.Sum()+modifier;

            // Build the roll details string
            string rollDetails = $"{string.Join(", ", individualRolls)}"; ;
            if (modifier != 0)
                rollDetails += $" + Modifier({modifier})";
            rollDetails += $" = {total}";

            string diceDetails = $"Dice Used: {string.Join(", ", selectedDice.Select(d => $"{d.Value}d{d.Key}")) }";

            // Combine the roll results and dice used into RollDetails
            rollDetails += $"\n{diceDetails}";

            // Create a new RollHistoryItem with the details
            var rollItem = new RollHistoryItem
            {
                User = _username,
                Total = total,
                IndividualRolls = individualRolls,
                RollDetails = rollDetails, // This includes everything
                Timestamp = DateTime.Now.ToString("g"),
                DiceUsed = selectedDice // Still keep this for future use
            };

            // Add the item to the roll history
            _rollHistoryManager.AddRollHistoryItem(rollItem);

            // Send the roll over SignalR or other communication methods
            await _signalRService.SendRoll(_username, total, individualRolls, selectedDice, modifier);
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

            var rollDetails = $"Rolls: {string.Join(", ", e.IndividualRolls)}";
            rollDetails += $" = {e.Total}"; // Show the total result

            // Add the dice used information
            string diceDetails = $"Dice Used: {string.Join(", ", e.DiceUsed.Select(d => $"{d.Value}d{d.Key}"))}";

            // Combine the roll results and dice used into RollDetails
            rollDetails += $"\n{diceDetails}";

            var rollItem = new RollHistoryItem
            {
                User = e.User,
                Total = e.Total,
                IndividualRolls = e.IndividualRolls,
                RollDetails = rollDetails,
                Timestamp = DateTime.Now.ToString("g"),
                DiceUsed = e.DiceUsed  // Assign the DiceUsed property
            };

            MainThread.BeginInvokeOnMainThread(() =>
            {
                _rollHistoryManager.AddRollHistoryItem(rollItem);
                DisplayAlert("New Roll", $"{e.User} rolled:\n{rollDetails}", "OK");
            });
        }
    }
}