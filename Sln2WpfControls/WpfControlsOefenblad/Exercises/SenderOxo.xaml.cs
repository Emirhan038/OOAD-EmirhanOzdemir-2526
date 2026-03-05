using System.Windows;
using System.Windows.Controls;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Sender Oxo", Description = "OXO spel met één click event handler", Order = 8, IsVisible = true)]
    public partial class SenderOxo : Page
    {
        private bool isX = true; 

        public SenderOxo()
        {
            InitializeComponent();
        }

        private void OxoButton_Click(object sender, RoutedEventArgs e)
        {
            Button geklikt = (Button)sender;

 
            geklikt.Content = isX ? "X" : "O";

 
            geklikt.IsEnabled = false;

            isX = !isX;
        }
    }
}