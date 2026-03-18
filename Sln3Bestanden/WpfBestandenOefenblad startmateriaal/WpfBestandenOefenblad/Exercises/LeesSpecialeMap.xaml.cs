using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WpfBestandenOefenblad.Helpers;

namespace WpfBestandenOefenblad.Exercises;

[NavPage(Title = "Lees speciale map", Description = "Drie knoppen: Desktop, Documenten, Afbeeldingen; bestanden met grootte en aanmaakdatum in TextBlock", Order = 6, IsVisible = true)]
public partial class LeesSpecialeMap : Page
{
    public LeesSpecialeMap()
    {
        InitializeComponent();
    }
}
