using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using WpfBestandenOefenblad.Helpers;

namespace WpfBestandenOefenblad.Exercises;

[NavPage(Title = "Lees CSV", Description = "CSV inlezen (Product;Quantity;Price), regels tonen en totaal verkoopbedrag", Order = 2, IsVisible = true)]
public partial class LeesCsv : Page
{
    public LeesCsv()
    {
        InitializeComponent();
    }
}
