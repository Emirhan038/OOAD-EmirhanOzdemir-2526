using System.Windows;
using System.Windows.Controls;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Select And Move", Description = "Items verplaatsen tussen twee ListBoxes.", Order = 9, IsVisible = true)]
    public partial class SelectAndMove : Page
    {
        public SelectAndMove()
        {
            InitializeComponent();
        }

        private void btnToSelected_Click(object sender, RoutedEventArgs e)
        {
            if (lstBeschikbaar.SelectedItem is not ListBoxItem item) return;

            lstBeschikbaar.Items.Remove(item);
            lstGeselecteerd.Items.Add(item);
        }

        private void btnToAvailable_Click(object sender, RoutedEventArgs e)
        {
            if (lstGeselecteerd.SelectedItem is not ListBoxItem item) return;

            lstGeselecteerd.Items.Remove(item);
            lstBeschikbaar.Items.Add(item);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnToSelected.IsEnabled = lstBeschikbaar.SelectedItem != null;
            btnToAvailable.IsEnabled = lstGeselecteerd.SelectedItem != null;
        }
    }
}