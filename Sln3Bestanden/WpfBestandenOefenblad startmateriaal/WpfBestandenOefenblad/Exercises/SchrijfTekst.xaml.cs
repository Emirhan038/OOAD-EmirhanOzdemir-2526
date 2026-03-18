using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using WpfBestandenOefenblad.Helpers;

namespace WpfBestandenOefenblad.Exercises;

[NavPage(Title = "Schrijf tekst", Description = "Tekst intikken in een TextBox en opslaan naar een bestand", Order = 4, IsVisible = true)]
public partial class SchrijfTekst : Page
{
    public SchrijfTekst()
    {
        InitializeComponent();
    }
}
