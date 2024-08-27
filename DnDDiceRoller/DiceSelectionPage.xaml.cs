using Microsoft.Maui.Controls;
using System;

namespace DnDDiceRoller
{
    public partial class DiceSelectionPage : ContentPage
    {
        private int diceSides;

        public event Action<int, int> DiceSelected;

        public DiceSelectionPage(int sides)
        {
            InitializeComponent();
            diceSides = sides;
        }

        private async void AddButton_Clicked(object sender, EventArgs e)
        {
            if (int.TryParse(QuantityEntry.Text, out int quantity))
            {
                DiceSelected?.Invoke(diceSides, quantity);
                await Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("Error", "Please enter a valid quantity.", "OK");
            }
        }
    }
}
