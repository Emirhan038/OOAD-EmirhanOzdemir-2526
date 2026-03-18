using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WpfBestandenOefenblad.Helpers;

namespace WpfBestandenOefenblad.Exercises;

[NavPage(Title = "Pad builder", Description = "Paden samenstellen uit basispad, map en bestandsnaam", Order = 1, IsVisible = true)]
public partial class PadBuilder : Page
{
    public PadBuilder()
    {
        InitializeComponent();
    }

    private void btnGenereerPad_Click(object sender, RoutedEventArgs e)
    {

    }
}
