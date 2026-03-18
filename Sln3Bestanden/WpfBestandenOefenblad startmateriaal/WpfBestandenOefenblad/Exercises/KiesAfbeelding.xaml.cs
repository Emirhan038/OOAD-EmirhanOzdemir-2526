using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using WpfBestandenOefenblad.Helpers;

namespace WpfBestandenOefenblad.Exercises;

[NavPage(Title = "Kies afbeelding", Description = "OpenFileDialog om een jpg/jpeg te kiezen en in een Image te tonen", Order = 3, IsVisible = true)]
public partial class KiesAfbeelding : Page
{
    public KiesAfbeelding()
    {
        InitializeComponent();
    }
}
