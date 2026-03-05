using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises;

[NavPage(Title = "Language Selector", Description = "ComboBox SelectionChanged event en ComboBoxItem", Order = 6, IsVisible = true)]
public partial class LanguageSelector : Page
{
    public LanguageSelector()
    {
        InitializeComponent();

        string[] languages = { "Nederlands", "English", "Français" };

        foreach (string taal in languages)
        {
            cmbTaal.Items.Add(new ComboBoxItem { Content = taal });
        }
    }

    private void cmbTaal_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (txtBegroeting == null) return;


        if (cmbTaal.SelectedItem is not ComboBoxItem geselecteerd) return;

        string taal = geselecteerd.Content.ToString();

        foreach (ComboBoxItem item in cmbTaal.Items)
        {
            item.FontWeight = FontWeights.Normal;
        }

        geselecteerd.FontWeight = FontWeights.Bold;


        txtBegroeting.Text = taal switch
        {
            "Nederlands" => "Hallo",
            "English" => "Hello",
            "Français" => "Bonjour",
            _ => ""
        };
    }
}