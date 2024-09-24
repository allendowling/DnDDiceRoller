
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnDDiceRoller;


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
            //Button animation
            Button button = (Button)sender;

            await button.ScaleTo(0.9, 10, Easing.CubicOut);
            await button.ScaleTo(1, 10, Easing.CubicIn);
            // Get the username from the entry
            _username = UsernameEntry.Text?.Trim();
            // Check if the username is valid
            if (string.IsNullOrWhiteSpace(_username))
            {
                await DisplayAlert("Error", "Please enter a valid username.", "OK");
                return;
            }
            // Hide the login section and show the main app section
            LoginSection.IsVisible = false;
            MainAppSection.IsVisible = true;
            // Initialize the SignalR service
            try
            {
                await _signalRService.InitializeAsync("https://dnddicerollerapi20240821154836.azurewebsites.net/rollHub");
            }
            // Handle any exceptions
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not connect to server: {ex.Message}", "OK");
            }
        }

        private async void AddDiceButton_Clicked(object sender, EventArgs e)
        {
            //Button animation
            Button button = (Button)sender;

            await button.ScaleTo(0.9, 50, Easing.CubicOut);
            await button.ScaleTo(1, 50, Easing.CubicIn);
            // Get the selected dice type and quantity
            string selectedDiceType = DiceTypePicker.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(selectedDiceType) || string.IsNullOrWhiteSpace(DiceQuantityEntry.Text))
            // Check if the dice type and quantity are selected
            {
               await DisplayAlert("Error", "Please select a dice type and enter a quantity.", "OK");
                return;
            }
            
            if (!int.TryParse(DiceQuantityEntry.Text, out int quantity) || quantity <= 0)
            {// Check if the quantity is valid
                await DisplayAlert("Error", "Please enter a valid quantity.", "OK");// Show an error message
                return;
            }
            // Extract sides from the dice type string
            int sides = int.Parse(selectedDiceType.Substring(1)); // Extract sides from the dice type string
            // Add the selected dice to the dictionary
            if (selectedDice.ContainsKey(sides))
            {
                selectedDice[sides] += quantity;
            }
            else
            {
                selectedDice[sides] = quantity;
            }
            // Update the selected dice list
            UpdateSelectedDiceList();
            DiceQuantityEntry.Text = string.Empty;
        }
        // Update the selected dice list
        private void UpdateSelectedDiceList()
        {
            var diceList = selectedDice.Select(d => $"{d.Value}d{d.Key}").ToList();
            SelectedDiceCollectionView.ItemsSource = diceList;
        }
        // Handle the roll button click event
        private async void RollButton_Clicked(object sender, EventArgs e)
        {
            //Button animation
            Button button = (Button)sender;

            await button.ScaleTo(0.9, 50, Easing.CubicOut);
            await button.ScaleTo(1, 50, Easing.CubicIn);

            int modifier = int.TryParse(ModifierEntry.Text, out var mod) ? mod : 0;

            // Roll the dice
            var individualRolls = diceRoller.RollMultipleDice(selectedDice, modifier);
            int total = individualRolls.Sum()+modifier;

            // Build the roll details string
            string rollDetails = $"{string.Join(", ", individualRolls)}"; ;
            if (modifier != 0)
                rollDetails += $" + Modifier({modifier})";
            rollDetails += $" = {total}";
            // Add the dice used information
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
        // Handle the clear dice button click event
        private async void ClearDiceButton_Clicked(object sender, EventArgs e)
        {
            //Button animation
            Button button = (Button)sender;
            await button.ScaleTo(0.9, 50, Easing.CubicOut);
            await button.ScaleTo(1, 50, Easing.CubicIn);

            // Clear the selected dice and update the list
            selectedDice.Clear();
            UpdateSelectedDiceList();
            ModifierEntry.Text = string.Empty;
        }
       
        // Handle the roll received event
        private void SignalRService_RollReceived(object sender, RollEventArgs e)
        {   // Check if the roll was sent by the current user
            if (e.User == _username)
                return; // Ignore own rolls
            // Build the roll details string
            var rollDetails = $"Rolls: {string.Join(", ", e.IndividualRolls)}";
            rollDetails += $" = {e.Total}"; // Show the total result

            // Add the dice used information
            string diceDetails = $"Dice Used: {string.Join(", ", e.DiceUsed.Select(d => $"{d.Value}d{d.Key}"))}";

            // Combine the roll results and dice used into RollDetails
            rollDetails += $"\n{diceDetails}";
            // Create a new RollHistoryItem with the details
            var rollItem = new RollHistoryItem
            {
                User = e.User,
                Total = e.Total,
                IndividualRolls = e.IndividualRolls,
                RollDetails = rollDetails,
                Timestamp = DateTime.Now.ToString("g"),
                DiceUsed = e.DiceUsed  // Assign the DiceUsed property
            };
            // Add the item to the roll history
            MainThread.BeginInvokeOnMainThread(() =>
            {
                _rollHistoryManager.AddRollHistoryItem(rollItem);
                DisplayAlert("New Roll", $"{e.User} rolled:\n{rollDetails}", "OK");
            });


        }
       
    }
}