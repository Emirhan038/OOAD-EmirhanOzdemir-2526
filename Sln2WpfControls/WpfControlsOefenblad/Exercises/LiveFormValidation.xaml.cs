using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Live Form Validation", Description = "TextChanged gebruiken voor live validatie en IsEnabled.", Order = 4, IsVisible = true)]
    public partial class LiveFormValidation : Page
    {
        public LiveFormValidation()
        {
            InitializeComponent();
        }

        private void txtPaswoord_TextChanged(object sender, TextChangedEventArgs e)
        {
            string password = txtPaswoord.Text;

            if (string.IsNullOrWhiteSpace(password))
            {
                txtStatus.Text = "...";
                txtStatus.Foreground = Brushes.Black;
                btnSave.IsEnabled = false;
                return;
            }

            var fouten = new System.Collections.Generic.List<string>();

            if (password.Length < 8)
                fouten.Add("Minstens 8 tekens vereist.");
            if (!password.Any(c => char.IsUpper(c)))
                fouten.Add("Minstens één hoofdletter vereist.");
            if (!password.Any(c => char.IsDigit(c)))
                fouten.Add("Minstens één cijfer vereist.");

            if (fouten.Count > 0)
            {
                txtStatus.Text = "Ongeldig paswoord:\n" + string.Join("\n", fouten);
                txtStatus.Foreground = Brushes.Red;
                btnSave.IsEnabled = false;
            }
            else
            {
                txtStatus.Text = "Geldig paswoord";
                txtStatus.Foreground = Brushes.DarkGreen;
                btnSave.IsEnabled = true;
            }
        }
    }
}