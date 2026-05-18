using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BusinessLayer;

namespace PatientApp;

/// <summary>
/// Hoofdvenster van de patiëntenapp.
/// Bevat het linkermenu en een Frame rechts dat Page-objecten toont.
/// </summary>
public partial class MainWindow : Window
{
    // Ingelogde patiënt; null zolang niemand ingelogd is.
    private Patient _ingelogdePatient = null;

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
    public void VerwerkInloggen(Patient patient)
    {
        _ingelogdePatient = patient;

        // Toon volledige naam in de topbalk.
        TxtGebruikerNaam.Text = patient.Voornaam + " " + patient.Achternaam;

        // Laad profielfoto als die aanwezig is; anders blijft de placeholder zichtbaar.
        if (patient.Profielfotodata != null && patient.Profielfotodata.Length > 0)
        {
            try
            {
                // Converteer de raw bytes naar een BitmapImage via een MemoryStream.
                BitmapImage bitmap = new BitmapImage();
                MemoryStream ms = new MemoryStream(patient.Profielfotodata);
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

        // Activeer de patiëntspecifieke menuknoppen.
        BtnAfspraken.IsEnabled = true;
        BtnProfiel.IsEnabled   = true;

        // Navigeer naar de afsprakenpagina als startpunt na inloggen.
        ToonPagina(new AfsprakenPage(_ingelogdePatient));
    }

    // ── Topbalk verversen (aangeroepen door ProfielPage na opslaan) ────────────

    /// <summary>
    /// Werkt de naam en profielfoto in de topbalk bij na een profielwijziging.
    /// </summary>
    public void RefreshTopbar(Patient patient)
    {
        _ingelogdePatient = patient;
        TxtGebruikerNaam.Text = patient.Voornaam + " " + patient.Achternaam;

        if (patient.Profielfotodata != null && patient.Profielfotodata.Length > 0)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                MemoryStream ms    = new MemoryStream(patient.Profielfotodata);
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.CacheOption  = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                ImgProfiel.Source            = bitmap;
                ImgProfiel.Visibility        = Visibility.Visible;
                BorderPlaceholder.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                ImgProfiel.Visibility        = Visibility.Collapsed;
                BorderPlaceholder.Visibility = Visibility.Visible;
            }
        }
        else
        {
            ImgProfiel.Visibility        = Visibility.Collapsed;
            BorderPlaceholder.Visibility = Visibility.Visible;
        }
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

    /// <summary>Toont de afsprakenpagina (alleen beschikbaar na inloggen).</summary>
    private void BtnAfspraken_Click(object sender, RoutedEventArgs e)
    {
        if (_ingelogdePatient != null)
        {
            ToonPagina(new AfsprakenPage(_ingelogdePatient));
        }
    }

    /// <summary>Toont de profielpagina (alleen beschikbaar na inloggen).</summary>
    private void BtnProfiel_Click(object sender, RoutedEventArgs e)
    {
        if (_ingelogdePatient != null)
        {
            ToonPagina(new ProfielPage(_ingelogdePatient));
        }
    }
}
