using System;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Animal sounds", Description = "Afbeeldingen van dieren en hun geluiden", Order = 10, IsVisible = true)]
    public partial class AnimalSounds : Page
    {
        public AnimalSounds()
        {
            InitializeComponent();
        }

        private void Image_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image theImage = sender as Image;
            string path = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "dieren geluiden",
                theImage.Tag.ToString()
            );

            if (!File.Exists(path)) return; 

            SoundPlayer player = new SoundPlayer(path);
            player.Play();
        }
    }
}