using System.Windows;
using System.Windows.Controls;
using WpfLayoutUIOefenblad.Helpers;

namespace WpfLayoutUIOefenblad.Exercises;

[NavPage(title: "Notepad App", description: "Menu en Statusbar in een \ngeïntegreerde oefening", order: 11)]
public partial class NotepadApp : Page
{
    public NotepadApp()
    {
        InitializeComponent();
    }

    private void MenuNew_Click(object sender, RoutedEventArgs e)
    {
        var tab = new TabItem { Header = "Untitled.txt" };
        tab.Content = new TextBox { AcceptsReturn = true, TextWrapping = TextWrapping.Wrap };
        tabControl.Items.Add(tab);
        tabControl.SelectedItem = tab;
    }

    private void MenuExit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void MenuAbout_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "WPF NotePad 1.0\n\nOntwikkeld door Safi Harare. Alle rechten voorbehouden.",
            "About WPF Notepad",
            MessageBoxButton.OK,
            MessageBoxImage.Information
        );
    }
}