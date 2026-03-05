using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Order Builder", Description = "CheckBox + RadioButton met samenvatting en reset.", Order = 5, IsVisible = true)]
    public partial class OrderBuilder : Page
    {
        public OrderBuilder()
        {
            InitializeComponent();
        }

        private void btnBevestig_Click(object sender, RoutedEventArgs e)
        {

            string levering = "";
            if (rbAfhalen.IsChecked == true) levering = "Afhalen";
            else if (rbLevering.IsChecked == true) levering = "Levering";
            else if (rbTerPlaatse.IsChecked == true) levering = "Ter plaatse";

            if (string.IsNullOrEmpty(levering))
            {
                txtSummary.Text = "Kies eerst een leveringsmethode";
                return;
            }


            var extras = new List<string>();
            if (chkKaas.IsChecked == true) extras.Add("Kaas");
            if (chkSpek.IsChecked == true) extras.Add("Spek");
            if (chkExtraSaus.IsChecked == true) extras.Add("Extra saus");
            if (chkUi.IsChecked == true) extras.Add("Ui");

            string extrasText = extras.Count > 0 ? string.Join(", ", extras) : "geen";
            txtSummary.Text = $"Levering: {levering}\nExtra's: {extrasText}";
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            chkKaas.IsChecked = true;
            chkSpek.IsChecked = false;
            chkExtraSaus.IsChecked = true;
            chkUi.IsChecked = false;

            rbAfhalen.IsChecked = false;
            rbLevering.IsChecked = false;
            rbTerPlaatse.IsChecked = false;

            txtSummary.Text = "...";
        }
    }
}