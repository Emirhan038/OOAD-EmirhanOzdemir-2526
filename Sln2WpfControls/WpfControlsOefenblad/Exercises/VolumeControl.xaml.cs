using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Volume Control", Description = "Slider gebruiken met ValueChanged en property-aanpassing.", Order = 7, IsVisible = true)]
    public partial class VolumeControl : Page
    {
        public VolumeControl()
        {
            InitializeComponent();
        }

        private void sldVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (txtVolume == null || brdVolume == null) return;

            int volume = (int)sldVolume.Value;

            txtVolume.Text = $"Volume: {volume}%";

            brdVolume.Background = volume switch
            {
                < 20 => Brushes.Green,
                < 40 => Brushes.Yellow,
                < 60 => Brushes.Orange,
                < 80 => Brushes.Red,
                _ => Brushes.DarkRed
            };

            brdVolume.Width = stpVolumeControl.ActualWidth * volume / 100.0;
        }
    }
}
