using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BusinessLayer;
using Microsoft.Win32;

namespace DokterApp;

/// <summary>
/// Formulierpagina voor zowel het toevoegen als het wijzigen van een patiënt.
/// Wanneer _patient null is, worden lege velden getoond en wordt een nieuw record aangemaakt.
/// Wanneer _patient een bestaande patiënt is, worden de velden vooraf ingevuld.
/// </summary>
public partial class PatientFormPage : Page
{
    // Referentie naar de shell-pagina voor navigatie.
    private PatientenPage _parent;

    // De te bewerken patiënt; null als het om een nieuwe patiënt gaat.
    private Patient _patient;

    // Ingelogde dokter — meegestuurd bij terugnavigatie.
    private Dokter _dokter;

    // Ruwe bytes van de gekozen profielfoto; null als er geen foto gekozen is.
    private byte[] _fotoData = null;

    // True = nieuwe patiënt; False = bestaande patiënt bewerken.
    private bool _isNieuw;

    /// <summary>
    /// Ontvangt de patiënt (of null voor nieuw), vult het formulier in
    /// en configureert de titel en de modus.
    /// </summary>
    public PatientFormPage(PatientenPage parent, Patient patient, Dokter dokter)
    {
        InitializeComponent();
        _parent  = parent;
        _patient = patient;
        _dokter  = dokter;
        _isNieuw = (_patient == null);

        VulNotificatiesComboBox();
        CalGeboortedatum.SelectedDatesChanged += CalGeboortedatum_SelectedDatesChanged!;

        if (_isNieuw)
        {
            TxtTitel.Text = "Nieuwe patiënt toevoegen";
            // Standaard geboortedatum: vandaag
            CalGeboortedatum.SelectedDate = DateTime.Today;
        }
        else
        {
            TxtTitel.Text = "Patiënt wijzigen";
            VulFormulierIn();
        }
    }

    // ── Formulier initialiseren ───────────────────────────────────────────────

    /// <summary>Voegt de vier notificatieopties toe aan de ComboBox.</summary>
    private void VulNotificatiesComboBox()
    {
        CmbNotificaties.Items.Add("Geen");
        CmbNotificaties.Items.Add("Mail");
        CmbNotificaties.Items.Add("Sms");
        CmbNotificaties.Items.Add("Beide");
        CmbNotificaties.SelectedIndex = 0; // standaard: Geen
    }

    /// <summary>
    /// Vult het formulier in met de gegevens van de bestaande patiënt.
    /// Geslacht en Notificaties worden via expliciete if/else gemapt — nooit via SelectedIndex.
    /// </summary>
    private void VulFormulierIn()
    {
        TxtVoornaam.Text   = _patient.Voornaam;
        TxtAchternaam.Text = _patient.Achternaam;
        TxtEmail.Text      = _patient.Email;
        TxtGsm.Text        = _patient.Gsm ?? "";

        // Geboortedatum instellen op de kalender.
        CalGeboortedatum.SelectedDate = _patient.Geboortedatum;

        // Geslacht: expliciete if/else op de enum-waarde (NOOIT SelectedIndex of switch-patronen).
        if (_patient.Geslacht == Geslacht.Man)
            RbMan.IsChecked = true;
        else if (_patient.Geslacht == Geslacht.Vrouw)
            RbVrouw.IsChecked = true;
        else if (_patient.Geslacht == Geslacht.NietVanToepassing)
            RbNietVanToepassing.IsChecked = true;
        else
            RbOnbekend.IsChecked = true;

        // Notificatie: expliciete if/else op de enum-waarde.
        if (_patient.Notificaties == Notificatie.Mail)
            CmbNotificaties.SelectedIndex = 1;
        else if (_patient.Notificaties == Notificatie.Sms)
            CmbNotificaties.SelectedIndex = 2;
        else if (_patient.Notificaties == Notificatie.Beide)
            CmbNotificaties.SelectedIndex = 3;
        else
            CmbNotificaties.SelectedIndex = 0;

        // Bestaande foto bewaren en als preview tonen.
        _fotoData = _patient.Profielfotodata;
        ToonFotoPreview();
    }

    // ── Kalender-event ────────────────────────────────────────────────────────

    /// <summary>Toont de geselecteerde geboortedatum als leesbare tekst onder de kalender.</summary>
    private void CalGeboortedatum_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CalGeboortedatum.SelectedDate != null)
        {
            TxtGeselecteerdeDatum.Text = "Geselecteerd: " +
                CalGeboortedatum.SelectedDate.Value.ToString("dd/MM/yyyy");
        }
        else
        {
            TxtGeselecteerdeDatum.Text = "";
        }
    }

    // ── Fotokiezer ────────────────────────────────────────────────────────────

    /// <summary>
    /// Opent een OpenFileDialog, leest de geselecteerde afbeelding als byte[]
    /// en toont een preview — fouten gaan naar TxtFout.
    /// </summary>
    private void BtnKiesFoto_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dlg = new OpenFileDialog();
        dlg.Title  = "Kies een profielfoto";
        dlg.Filter = "Afbeeldingen|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

        bool? resultaat = dlg.ShowDialog();
        if (resultaat != true)
        {
            return;
        }

        try
        {
            _fotoData = File.ReadAllBytes(dlg.FileName);
            ToonFotoPreview();
        }
        catch (Exception ex)
        {
            ToonFout("Fout bij laden van foto: " + ex.Message);
        }
    }

    /// <summary>
    /// Toont de huidig geselecteerde foto als BitmapImage in ImgFotoPreview.
    /// Bij een fout of als er geen foto is, wordt de preview verborgen.
    /// </summary>
    private void ToonFotoPreview()
    {
        if (_fotoData == null || _fotoData.Length == 0)
        {
            ImgFotoPreview.Visibility = Visibility.Collapsed;
            return;
        }

        try
        {
            BitmapImage bmp = new BitmapImage();
            MemoryStream ms  = new MemoryStream(_fotoData);
            bmp.BeginInit();
            bmp.StreamSource = ms;
            bmp.CacheOption  = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            ImgFotoPreview.Source     = bmp;
            ImgFotoPreview.Visibility = Visibility.Visible;
        }
        catch (Exception)
        {
            ImgFotoPreview.Visibility = Visibility.Collapsed;
        }
    }

    // ── Opslaan ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Valideert het formulier en slaat de patiënt op (nieuw of bijgewerkt).
    /// Bij een validatie- of databasefout wordt een melding getoond in TxtFout.
    /// </summary>
    private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
    {
        VerbergFout();

        if (!ValideerFormulier())
        {
            return;
        }

        // ── Velden uitlezen ────────────────────────────────────────────────

        string voornaam   = TxtVoornaam.Text.Trim();
        string achternaam = TxtAchternaam.Text.Trim();
        string email      = TxtEmail.Text.Trim();
        string gsm        = TxtGsm.Text.Trim();

        // Geslacht: expliciete if/else op de RadioButton-waarden.
        Geslacht geslacht;
        if (RbMan.IsChecked == true)
            geslacht = Geslacht.Man;
        else if (RbVrouw.IsChecked == true)
            geslacht = Geslacht.Vrouw;
        else if (RbNietVanToepassing.IsChecked == true)
            geslacht = Geslacht.NietVanToepassing;
        else
            geslacht = Geslacht.Onbekend;

        // Notificatie: expliciete if/else op de ComboBox-tekst.
        string notificatieTekst = CmbNotificaties.SelectedItem.ToString();
        Notificatie notificatie;
        if (notificatieTekst == "Mail")
            notificatie = Notificatie.Mail;
        else if (notificatieTekst == "Sms")
            notificatie = Notificatie.Sms;
        else if (notificatieTekst == "Beide")
            notificatie = Notificatie.Beide;
        else
            notificatie = Notificatie.Geen;

        // ValideerFormulier() heeft al gecontroleerd dat SelectedDate niet null is.
        DateTime geboortedatum = CalGeboortedatum.SelectedDate!.Value;

        // ── Nieuw of bijwerken ─────────────────────────────────────────────

        if (_isNieuw)
        {
            Patient nieuw = new Patient();
            nieuw.Voornaam       = voornaam;
            nieuw.Achternaam     = achternaam;
            nieuw.Email          = email;
            nieuw.Gsm            = gsm.Length > 0 ? gsm : null;
            nieuw.Geslacht       = geslacht;
            nieuw.Geboortedatum  = geboortedatum;
            nieuw.Notificaties   = notificatie;
            nieuw.Profielfotodata = _fotoData;

            // Nieuwe patiënten zonder wachtwoord krijgen SHA-256("test").
            nieuw.Paswoord = Persoon.HashPaswoord("test");

            try
            {
                nieuw.Opslaan();
            }
            catch (Exception ex)
            {
                ToonFout("Fout bij opslaan patiënt: " + ex.Message);
                return;
            }
        }
        else
        {
            _patient.Voornaam        = voornaam;
            _patient.Achternaam      = achternaam;
            _patient.Email           = email;
            _patient.Gsm             = gsm.Length > 0 ? gsm : null;
            _patient.Geslacht        = geslacht;
            _patient.Geboortedatum   = geboortedatum;
            _patient.Notificaties    = notificatie;
            _patient.Profielfotodata = _fotoData;
            // Paswoord blijft ongewijzigd bij het bewerken.

            try
            {
                _patient.Bijwerken();
            }
            catch (Exception ex)
            {
                ToonFout("Fout bij bijwerken patiënt: " + ex.Message);
                return;
            }
        }

        // Terug naar het overzicht na succesvol opslaan.
        _parent.NavigeerNaar(new PatientenOverzichtPage(_parent, _dokter));
    }

    // ── Validatie ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Controleert alle verplichte velden.
    /// Toont een foutmelding in TxtFout bij een probleem (nooit MessageBox).
    /// Geeft true terug als alles in orde is.
    /// </summary>
    private bool ValideerFormulier()
    {
        if (TxtVoornaam.Text.Trim().Length == 0)
        {
            ToonFout("Voornaam is verplicht.");
            return false;
        }
        if (TxtAchternaam.Text.Trim().Length == 0)
        {
            ToonFout("Achternaam is verplicht.");
            return false;
        }
        if (TxtEmail.Text.Trim().Length == 0)
        {
            ToonFout("E-mailadres is verplicht.");
            return false;
        }
        // Basiscontrole op e-mailformaat: moet '@' en '.' bevatten.
        if (!TxtEmail.Text.Contains("@") || !TxtEmail.Text.Contains("."))
        {
            ToonFout("Voer een geldig e-mailadres in (bevat '@' en '.').");
            return false;
        }
        if (CalGeboortedatum.SelectedDate == null)
        {
            ToonFout("Selecteer een geboortedatum.");
            return false;
        }
        if (CmbNotificaties.SelectedItem == null)
        {
            ToonFout("Selecteer een notificatievoorkeur.");
            return false;
        }
        return true;
    }

    // ── Navigatie ─────────────────────────────────────────────────────────────

    /// <summary>Navigeert terug naar het patiëntenoverzicht zonder op te slaan.</summary>
    private void BtnTerug_Click(object sender, RoutedEventArgs e)
    {
        _parent.NavigeerNaar(new PatientenOverzichtPage(_parent, _dokter));
    }

    // ── Hulpmethoden ──────────────────────────────────────────────────────────

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
