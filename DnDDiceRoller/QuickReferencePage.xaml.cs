using Microsoft.Maui.Controls;

namespace DnDDiceRoller
{
    public partial class QuickReferencePage : ContentPage
    {
        public QuickReferencePage()
        {
            InitializeComponent();

            // Set the URL to load
            string url = "https://crobi.github.io/dnd5e-quickref/preview/quickref.html";

            // Load the URL in the WebView
            QuickReferenceWebView.Source = url;
        }
    }
}