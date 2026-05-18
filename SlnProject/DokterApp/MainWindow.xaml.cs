using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BusinessLayer;

namespace DokterApp;

/// <summary>
/// Hoofdvenster van de artsendashboard-app.
/// Bevat het linkermenu en een Frame rechts dat Page-objecten toont.
/// </summary>
public partial class MainWindow : Window
{
    // Ingelogde dokter; null zolang niemand ingelogd is.
    private Dokter _ingelogdeDokter = null;

    /// <summary>Initialiseert het venster en toont de startpagina.</summary>
    public MainWindow()
    {
        InitializeComponent();
        // Toon de startpagina zodra het venster opent.
        ToonPagina(new StartPage());
    }

    // ── Navigatie ──────────────────────────────────────────────────────────────

    /// <summary>Laadt de opgegeven pagina in het centrale Frame.</summary>
    private void ToonPagina(Page pagina)
    {
        MainFrame.Content = pagina;
    }

    // ── Inlogcallback (aangeroepen door LoginPage) ──────────────────────────────

    /// <summary>
    /// Wordt aangeroepen door LoginPage nadat inloggen geslaagd is.
    /// Werkt de naam, profielfoto en menuknoppen bij en navigeert naar Afspraken.
    /// </summary>
    public void VerwerkInloggen(Dokter dokter)
    {
        _ingelogdeDokter = dokter;

        // Toon volledige naam in de topbalk.
        TxtGebruikerNaam.Text = dokter.Voornaam + " " + dokter.Achternaam;

        // Laad profielfoto als die aanwezig is; anders blijft de placeholder zichtbaar.
        if (dokter.Profielfotodata != null && dokter.Profielfotodata.Length > 0)
        {
            try
            {
                // Converteer de raw bytes naar een BitmapImage via een MemoryStream.
                BitmapImage bitmap = new BitmapImage();
                MemoryStream ms = new MemoryStream(dokter.Profielfotodata);
                bitmap.BeginInit();
                bitmap.StreamSource    = ms;
                bitmap.CacheOption     = BitmapCacheOption.OnLoad; // stream mag daarna gesloten worden
                bitmap.EndInit();

                ImgProfiel.Source         = bitmap;
                ImgProfiel.Visibility     = Visibility.Visible;
                BorderPlaceholder.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                // Ongeldig afbeeldingsformaat: toon de placeholder.
                ImgProfiel.Visibility        = Visibility.Collapsed;
                BorderPlaceholder.Visibility = Visibility.Visible;
            }
        }

        // Activeer de art-specifieke menuknoppen.
        BtnPatienten.IsEnabled = true;
        BtnAfspraken.IsEnabled = true;

        // Navigeer naar de afsprakenpagina als startpunt na inloggen.
        ToonPagina(new AfsprakenPage(_ingelogdeDokter));
    }

    // ── Menuknophandelaars ─────────────────────────────────────────────────────

    /// <summary>Toont de startpagina.</summary>
    private void BtnStart_Click(object sender, RoutedEventArgs e)
    {
        ToonPagina(new StartPage());
    }

    /// <summary>Toont de loginpagina; geeft 'this' mee zodat LoginPage kan terugkoppelen.</summary>
    private void BtnInloggen_Click(object sender, RoutedEventArgs e)
    {
        ToonPagina(new LoginPage(this));
    }

    /// <summary>Toont de patiëntenpagina (alleen beschikbaar na inloggen).</summary>
    private void BtnPatienten_Click(object sender, RoutedEventArgs e)
    {
        if (_ingelogdeDokter != null)
        {
            ToonPagina(new PatientenPage(_ingelogdeDokter));
        }
    }

    /// <summary>Toont de afsprakenpagina (alleen beschikbaar na inloggen).</summary>
    private void BtnAfspraken_Click(object sender, RoutedEventArgs e)
    {
        if (_ingelogdeDokter != null)
        {
            ToonPagina(new AfsprakenPage(_ingelogdeDokter));
        }
    }
}
