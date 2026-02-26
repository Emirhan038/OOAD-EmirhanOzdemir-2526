using System.Windows;
using System.Windows.Controls;
using WpfLayoutUIOefenblad.Helpers;

namespace WpfLayoutUIOefenblad.Exercises
{
    [NavPage(title: "MessageBox", description: "Dialoogvensters", order: 10)]
    public partial class MessageBoxDialog : Page
    {
        public MessageBoxDialog()
        {
            InitializeComponent();

        }

        private void btnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Wil je de gegevens opslaan?",
                "Opslaan",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No   
            );

            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show(
                    "De gegevens zijn opgeslagen.",
                    "Opgeslagen",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }
    }
}