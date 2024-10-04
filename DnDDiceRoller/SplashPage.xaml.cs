using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace DnDDiceRoller
{
    public partial class SplashPage : ContentPage
    {
        public SplashPage()
        {
            InitializeComponent();

            // Start the timer to navigate to the main page
            NavigateToMainPage();
        }

        private async void NavigateToMainPage()
        {
            // Set the duration (in milliseconds) for the splash screen
            await Task.Delay(2000); // 3000 milliseconds = 3 seconds

            // Navigate to the main page
            Application.Current.MainPage = new AppShell();
        }
    }
}