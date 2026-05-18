using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BusinessLayer;

namespace DokterApp;

/// <summary>
/// Overzichtspagina van het patiëntenbeheer.
/// Toont een zoekbalk en een raster van patiëntenkaarten gebouwd in code-behind
/// (geen DataGrid, geen binding, geen LINQ).
/// </summary>
public partial class PatientenOverzichtPage : Page
{
    // Referentie naar de shell-pagina voor navigatie naar subpagina's.
    private PatientenPage _parent;

    // Ingelogde dokter, meegestuurd naar subpagina's.
    private Dokter _dokter;

    // De patiënten die momenteel als kaarten worden getoond — nodig voor klikverwerking.
    private List<Patient> _huidigeLijst = new List<Patient>();

    /// <summary>
    /// Ontvangt de parent-pagina en de ingelogde dokter; laadt meteen alle patiënten.
    /// </summary>
    public PatientenOverzichtPage(PatientenPage parent, Dokter dokter)
    {
        InitializeComponent();
        _parent = parent;
        _dokter = dokter;

        // Laad alle patiënten bij het openen van de pagina.
        ZoekPatienten("");
    }

    // ── Zoeken ────────────────────────────────────────────────────────────────

    /// <summary>Verwerkt de zoekopdracht wanneer op "Zoeken" geklikt wordt.</summary>
    private void BtnZoeken_Click(object sender, RoutedEventArgs e)
    {
        ZoekPatienten(TxtZoeken.Text.Trim());
    }

    /// <summary>
    /// Haalt alle patiënten op uit de database, filtert op zoekterm (voor- of achternaam)
    /// en bouwt het kaartraster opnieuw op.
    /// Filtering gebeurt via foreach + .ToLower().Contains (geen LINQ).
    /// </summary>
    private void ZoekPatienten(string zoekterm)
    {
        VerbergFout();
        List<Patient> allePatienten = new List<Patient>();

        try
        {
            allePatienten = Patient.GetAll();
        }
        catch (Exception ex)
        {
            ToonFout("Fout bij ophalen patiënten: " + ex.Message);
            return;
        }

        // Filter op voornaam of achternaam — foreach, geen LINQ.
        _huidigeLijst = new List<Patient>();
        string zoekLower = zoekterm.ToLower();

        foreach (Patient patient in allePatienten)
        {
            if (zoekterm.Length == 0
                || patient.Voornaam.ToLower().Contains(zoekLower)
                || patient.Achternaam.ToLower().Contains(zoekLower))
            {
                _huidigeLijst.Add(patient);
            }
        }

        TxtAantal.Text = _huidigeLijst.Count + " patiënt(en) gevonden.";
        BouwKaarten(_huidigeLijst);
    }

    // ── Kaarten bouwen ────────────────────────────────────────────────────────

    /// <summary>
    /// Wist het WrapPanel en vult het opnieuw met één kaart per patiënt.
    /// Kaarten worden volledig in code-behind aangemaakt (geen XAML-templates).
    /// </summary>
    private void BouwKaarten(List<Patient> patienten)
    {
        PanelKaarten.Children.Clear();
        foreach (Patient patient in patienten)
        {
            Border kaart = BouwKaart(patient);
            PanelKaarten.Children.Add(kaart);
        }
    }

    /// <summary>
    /// Bouwt één patiëntenkaart: foto, naam, email, gsm en drie actieknopppen.
    /// De knop-Tag bevat patient.Id zodat de gedeelde klikhandler de patiënt kan terugvinden.
    /// </summary>
    private Border BouwKaart(Patient patient)
    {
        Border kaart = new Border();
        kaart.Width           = 230;
        kaart.Margin          = new Thickness(8);
        kaart.Padding         = new Thickness(14, 14, 14, 12);
        kaart.BorderThickness = new Thickness(1);
        kaart.BorderBrush     = new SolidColorBrush(Color.FromRgb(210, 218, 226));
        kaart.CornerRadius    = new CornerRadius(7);
        kaart.Background      = new SolidColorBrush(Colors.White);

        StackPanel inhoud = new StackPanel();

        // ── Profielfoto of placeholder ──────────────────────────────────────
        bool fotoGeladen = false;
        if (patient.Profielfotodata != null && patient.Profielfotodata.Length > 0)
        {
            try
            {
                BitmapImage bmp = new BitmapImage();
                MemoryStream ms  = new MemoryStream(patient.Profielfotodata);
                bmp.BeginInit();
                bmp.StreamSource = ms;
                bmp.CacheOption  = BitmapCacheOption.OnLoad;
                bmp.EndInit();

                Image img = new Image();
                img.Source              = bmp;
                img.Width               = 80;
                img.Height              = 80;
                img.Stretch             = Stretch.UniformToFill;
                img.HorizontalAlignment = HorizontalAlignment.Center;
                img.Margin              = new Thickness(0, 0, 0, 10);
                inhoud.Children.Add(img);
                fotoGeladen = true;
            }
            catch (Exception)
            {
                // Ongeldig afbeeldingsformaat — val door naar de placeholder.
            }
        }

        if (!fotoGeladen)
        {
            inhoud.Children.Add(MaakFotoPlaceholder());
        }

        // ── Naam ────────────────────────────────────────────────────────────
        TextBlock txtNaam = new TextBlock();
        txtNaam.Text                = patient.Voornaam + " " + patient.Achternaam;
        txtNaam.FontSize            = 14;
        txtNaam.FontWeight          = FontWeights.SemiBold;
        txtNaam.TextWrapping        = TextWrapping.Wrap;
        txtNaam.HorizontalAlignment = HorizontalAlignment.Center;
        txtNaam.Margin              = new Thickness(0, 0, 0, 4);
        inhoud.Children.Add(txtNaam);

        // ── E-mail ──────────────────────────────────────────────────────────
        TextBlock txtEmail = new TextBlock();
        txtEmail.Text                = patient.Email;
        txtEmail.FontSize            = 12;
        txtEmail.Foreground          = new SolidColorBrush(Color.FromRgb(86, 101, 115));
        txtEmail.TextWrapping        = TextWrapping.Wrap;
        txtEmail.HorizontalAlignment = HorizontalAlignment.Center;
        txtEmail.Margin              = new Thickness(0, 0, 0, 3);
        inhoud.Children.Add(txtEmail);

        // ── GSM — alleen tonen als ingevuld ─────────────────────────────────
        if (patient.Gsm != null && patient.Gsm.Length > 0)
        {
            TextBlock txtGsm = new TextBlock();
            txtGsm.Text                = patient.Gsm;
            txtGsm.FontSize            = 12;
            txtGsm.Foreground          = new SolidColorBrush(Color.FromRgb(86, 101, 115));
            txtGsm.HorizontalAlignment = HorizontalAlignment.Center;
            txtGsm.Margin              = new Thickness(0, 0, 0, 3);
            inhoud.Children.Add(txtGsm);
        }

        // ── Actieknoppen: Details / Wijzigen / Verwijderen ──────────────────
        StackPanel knoppen = new StackPanel();
        knoppen.Orientation          = Orientation.Horizontal;
        knoppen.HorizontalAlignment  = HorizontalAlignment.Center;
        knoppen.Margin               = new Thickness(0, 12, 0, 0);

        // Details-knop
        Button btnDetails = new Button();
        btnDetails.Content         = "Details";
        btnDetails.Tag             = patient.Id;     // id voor terugzoeken in klikhandler
        btnDetails.Margin          = new Thickness(2);
        btnDetails.Padding         = new Thickness(8, 4, 8, 4);
        btnDetails.FontSize        = 12;
        btnDetails.Background      = new SolidColorBrush(Color.FromRgb(41, 128, 185));
        btnDetails.Foreground      = new SolidColorBrush(Colors.White);
        btnDetails.BorderThickness = new Thickness(0);
        btnDetails.Cursor          = Cursors.Hand;
        btnDetails.Click          += BtnDetails_Click;   // gedeelde handler, geen lambda
        knoppen.Children.Add(btnDetails);

        // Wijzigen-knop
        Button btnWijzigen = new Button();
        btnWijzigen.Content         = "Wijzigen";
        btnWijzigen.Tag             = patient.Id;
        btnWijzigen.Margin          = new Thickness(2);
        btnWijzigen.Padding         = new Thickness(8, 4, 8, 4);
        btnWijzigen.FontSize        = 12;
        btnWijzigen.Background      = new SolidColorBrush(Color.FromRgb(39, 174, 96));
        btnWijzigen.Foreground      = new SolidColorBrush(Colors.White);
        btnWijzigen.BorderThickness = new Thickness(0);
        btnWijzigen.Cursor          = Cursors.Hand;
        btnWijzigen.Click          += BtnWijzigen_Click;
        knoppen.Children.Add(btnWijzigen);

        // Verwijderen-knop
        Button btnVerwijderen = new Button();
        btnVerwijderen.Content         = "Verwijderen";
        btnVerwijderen.Tag             = patient.Id;
        btnVerwijderen.Margin          = new Thickness(2);
        btnVerwijderen.Padding         = new Thickness(8, 4, 8, 4);
        btnVerwijderen.FontSize        = 12;
        btnVerwijderen.Background      = new SolidColorBrush(Color.FromRgb(231, 76, 60));
        btnVerwijderen.Foreground      = new SolidColorBrush(Colors.White);
        btnVerwijderen.BorderThickness = new Thickness(0);
        btnVerwijderen.Cursor          = Cursors.Hand;
        btnVerwijderen.Click          += BtnVerwijderen_Click;
        knoppen.Children.Add(btnVerwijderen);

        inhoud.Children.Add(knoppen);
        kaart.Child = inhoud;
        return kaart;
    }

    /// <summary>
    /// Maakt een grijze cirkel met "?" als placeholder wanneer er geen profielfoto is.
    /// </summary>
    private Border MaakFotoPlaceholder()
    {
        Border placeholder = new Border();
        placeholder.Width               = 80;
        placeholder.Height              = 80;
        placeholder.Background          = new SolidColorBrush(Color.FromRgb(174, 182, 191));
        placeholder.CornerRadius        = new CornerRadius(40);
        placeholder.HorizontalAlignment = HorizontalAlignment.Center;
        placeholder.Margin              = new Thickness(0, 0, 0, 10);

        TextBlock vraagteken = new TextBlock();
        vraagteken.Text                = "?";
        vraagteken.Foreground          = new SolidColorBrush(Colors.White);
        vraagteken.FontSize            = 32;
        vraagteken.HorizontalAlignment = HorizontalAlignment.Center;
        vraagteken.VerticalAlignment   = VerticalAlignment.Center;
        placeholder.Child = vraagteken;
        return placeholder;
    }

    // ── Klikhandlers voor actieknoppen ────────────────────────────────────────

    /// <summary>
    /// Gedeelde klikhandler voor alle "Details"-knoppen.
    /// Zoekt de patiënt op via button.Tag (= patient.Id) in _huidigeLijst.
    /// </summary>
    private void BtnDetails_Click(object sender, RoutedEventArgs e)
    {
        Button btn  = (Button)sender;
        int id      = (int)btn.Tag;
        Patient gevonden = ZoekPatientById(id);
        if (gevonden != null)
        {
            _parent.NavigeerNaar(new PatientDetailsPage(_parent, gevonden, _dokter));
        }
    }

    /// <summary>
    /// Gedeelde klikhandler voor alle "Wijzigen"-knoppen.
    /// Navigeert naar het formulier in bewerk-modus.
    /// </summary>
    private void BtnWijzigen_Click(object sender, RoutedEventArgs e)
    {
        Button btn  = (Button)sender;
        int id      = (int)btn.Tag;
        Patient gevonden = ZoekPatientById(id);
        if (gevonden != null)
        {
            _parent.NavigeerNaar(new PatientFormPage(_parent, gevonden, _dokter));
        }
    }

    /// <summary>
    /// Gedeelde klikhandler voor alle "Verwijderen"-knoppen.
    /// Navigeert naar de bevestigingspagina.
    /// </summary>
    private void BtnVerwijderen_Click(object sender, RoutedEventArgs e)
    {
        Button btn  = (Button)sender;
        int id      = (int)btn.Tag;
        Patient gevonden = ZoekPatientById(id);
        if (gevonden != null)
        {
            _parent.NavigeerNaar(new PatientVerwijderenPage(_parent, gevonden, _dokter));
        }
    }

    /// <summary>Navigeert naar het formulier in toevoeg-modus (null-patient = nieuw).</summary>
    private void BtnNieuwPatient_Click(object sender, RoutedEventArgs e)
    {
        _parent.NavigeerNaar(new PatientFormPage(_parent, null, _dokter));
    }

    // ── Hulpmethoden ──────────────────────────────────────────────────────────

    /// <summary>
    /// Zoekt een patiënt in _huidigeLijst via zijn id.
    /// Gebruikt foreach (geen LINQ); geeft null terug als niet gevonden.
    /// </summary>
    private Patient ZoekPatientById(int id)
    {
        foreach (Patient p in _huidigeLijst)
        {
            if (p.Id == id)
            {
                return p;
            }
        }
        return null;
    }

    /// <summary>Toont een foutmelding in de daarvoor voorziene TextBlock.</summary>
    private void ToonFout(string bericht)
    {
        TxtFout.Text       = bericht;
        TxtFout.Visibility = Visibility.Visible;
    }

    /// <summary>Verbergt de foutmelding.</summary>
    private void VerbergFout()
    {
        TxtFout.Text       = "";
        TxtFout.Visibility = Visibility.Collapsed;
    }
}
