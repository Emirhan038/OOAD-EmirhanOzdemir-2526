using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    /// <summary>
    /// Interaction logic for IntroPage.xaml
    /// </summary>
    [NavPage(Title ="Introductie", Description ="Uitleg over het oefenblad en werkwijze", Order=0)]
    public partial class IntroPage : Page
    {
        public IntroPage()
        {
            InitializeComponent();
        }
    }
}
