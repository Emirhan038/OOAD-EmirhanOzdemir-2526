using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using BusinessLayer;

namespace PatientApp;

/// <summary>
/// Profielpagina met twee panelen: gegevens bekijken en bewerken.
/// De ingelogde patiënt wordt via de constructor doorgegeven.
/// </summary>
public partial class ProfielPage : Page
{
    // Lokale kopie van de patiënt; wordt bijgewerkt na opslaan.
    private Patient _patient;

    // Houdt de nieuw gekozen fotobytes bij totdat de gebruiker op Opslaan klikt.
    private byte[] _nieuweFotoBytes = null;

    /// <summary>
    /// Ontvangt de ingelogde patiënt en toont meteen de profielgegevens.
    /// </summary>
    public ProfielPage(Patient patient)
    {
        InitializeComponent();
        _patient = patient;
        VulNotificatiesComboBox();
        ToonBekijkenPaneel();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  PANEEL 1 — BEKIJKEN
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Vult alle read-only velden met de actuele patiëntgegevens en toont het paneel.
    /// </summary>
    private void ToonBekijkenPaneel()
    {
        TxtFoutBekijk.Text = "";

        TxtVolledigeNaam.Text       = _patient.Voornaam + " " + _patient.Achternaam;
        TxtGeslachtBekijk.Text      = GeslachtNaarTekst(_patient.Geslacht);
        TxtGeboortedatumBekijk.Text = _patient.Geboortedatum.ToString("dd/MM/yyyy");
        TxtEmailBekijk.Text         = _patient.Email;
        TxtGsmBekijk.Text           = string.IsNullOrEmpty(_patient.Gsm) ? "—" : _patient.Gsm;
        TxtNotificatiesBekijk.Text  = NotificatieNaarTekst(_patient.Notificaties);

        // Profielfoto of placeholder tonen.
        LaadFotoInControl(_patient.Profielfotodata,
                          ImgFotoBekijk, BorderFotoPlaceholderBekijk);

        PanelBewerken.Visibility = Visibility.Collapsed;
        PanelBekijken.Visibility = Visibility.Visible;
    }

    /// <summary>Schakelt naar het bewerkpaneel en vult de invoervelden in.</summary>
    private void BtnBewerken_Click(object sender, RoutedEventArgs e)
    {
        ToonBewerkenPaneel();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  PANEEL 2 — BEWERKEN
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Vult de bewerkingsvelden met de huidige patiëntgegevens en toont het paneel.
    /// </summary>
    private void ToonBewerkenPaneel()
    {
        TxtFoutBew.Text  = "";
        _nieuweFotoBytes = null;
        TxtFotoNaam.Text = "";

        TxtVoornaam.Text   = _patient.Voornaam;
        TxtAchternaam.Text = _patient.Achternaam;
        TxtEmailBew.Text   = _patient.Email;
        TxtGsmBew.Text     = _patient.Gsm ?? "";

        // Geslacht instellen via if/else — geen LINQ, geen switch met pattern-matching.
        RbOnbekend.IsChecked         = false;
        RbMan.IsChecked              = false;
        RbVrouw.IsChecked            = false;
        RbNietVanToepassing.IsChecked = false;

        if (_patient.Geslacht == BusinessLayer.Geslacht.Man)
        {
            RbMan.IsChecked = true;
        }
        else if (_patient.Geslacht == BusinessLayer.Geslacht.Vrouw)
        {
            RbVrouw.IsChecked = true;
        }
        else if (_patient.Geslacht == BusinessLayer.Geslacht.NietVanToepassing)
        {
            RbNietVanToepassing.IsChecked = true;
        }
        else
        {
            RbOnbekend.IsChecked = true;
        }

        // Geboortedatum op de kalender selecteren.
        CalGeboortedatum.SelectedDate = _patient.Geboortedatum;

        // Notificatie-index in ComboBox instellen via if/else.
        if (_patient.Notificaties == BusinessLayer.Notificatie.Geen)
        {
            CmbNotificaties.SelectedIndex = 0;
        }
        else if (_patient.Notificaties == BusinessLayer.Notificatie.Mail)
        {
            CmbNotificaties.SelectedIndex = 1;
        }
        else if (_patient.Notificaties == BusinessLayer.Notificatie.Sms)
        {
            CmbNotificaties.SelectedIndex = 2;
        }
        else
        {
            CmbNotificaties.SelectedIndex = 3;
        }

        // Foto of placeholder in het bewerkpaneel.
        LaadFotoInControl(_patient.Profielfotodata,
                          ImgFotoBew, BorderFotoPlaceholderBew);

        PanelBekijken.Visibility = Visibility.Collapsed;
        PanelBewerken.Visibility = Visibility.Visible;
    }

    /// <summary>
    /// Opent een OpenFileDialog zodat de gebruiker een nieuwe profielfoto kan kiezen.
    /// Laadt de bytes in _nieuweFotoBytes en toont een voorbeeldweergave.
    /// </summary>
    private void BtnFotoKiezen_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dlg = new OpenFileDialog();
        dlg.Title  = "Kies een profielfoto";
        dlg.Filter = "Afbeeldingen|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Alle bestanden|*.*";

        if (dlg.ShowDialog() != true) return;

        try
        {
            _nieuweFotoBytes = File.ReadAllBytes(dlg.FileName);
            TxtFotoNaam.Text = Path.GetFileName(dlg.FileName);

            // Voorbeeld tonen in het bewerkpaneel.
            LaadFotoInControl(_nieuweFotoBytes, ImgFotoBew, BorderFotoPlaceholderBew);
        }
        catch (Exception ex)
        {
            TxtFoutBew.Text  = "Fout bij laden van de foto: " + ex.Message;
            _nieuweFotoBytes = null;
            TxtFotoNaam.Text = "";
        }
    }

    /// <summary>
    /// Valideert de invoer en slaat de gewijzigde profielgegevens op.
    /// Ververst daarna de topbalk en keert terug naar het bekijkpaneel.
    /// </summary>
    private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
    {
        TxtFoutBew.Text = "";

        string voornaam   = TxtVoornaam.Text.Trim();
        string achternaam = TxtAchternaam.Text.Trim();
        string email      = TxtEmailBew.Text.Trim();
        string gsm        = TxtGsmBew.Text.Trim();

        if (voornaam.Length == 0)
        {
            TxtFoutBew.Text = "Voornaam is verplicht.";
            return;
        }
        if (achternaam.Length == 0)
        {
            TxtFoutBew.Text = "Achternaam is verplicht.";
            return;
        }
        if (email.Length == 0)
        {
            TxtFoutBew.Text = "E-mailadres is verplicht.";
            return;
        }
        if (!CalGeboortedatum.SelectedDate.HasValue)
        {
            TxtFoutBew.Text = "Selecteer een geboortedatum.";
            return;
        }
        if (CmbNotificaties.SelectedIndex < 0)
        {
            TxtFoutBew.Text = "Kies een notificatievoorkeur.";
            return;
        }

        // Geslacht bepalen via if/else op RadioButton-staat.
        BusinessLayer.Geslacht geslacht = BusinessLayer.Geslacht.Onbekend;
        if (RbMan.IsChecked == true)
        {
            geslacht = BusinessLayer.Geslacht.Man;
        }
        else if (RbVrouw.IsChecked == true)
        {
            geslacht = BusinessLayer.Geslacht.Vrouw;
        }
        else if (RbNietVanToepassing.IsChecked == true)
        {
            geslacht = BusinessLayer.Geslacht.NietVanToepassing;
        }

        // Notificatie bepalen via if/else op ComboBox-tekst.
        BusinessLayer.Notificatie notificatie = BusinessLayer.Notificatie.Geen;
        string notTekst = CmbNotificaties.SelectedItem != null
            ? CmbNotificaties.SelectedItem.ToString()
            : "";

        if (notTekst == "Mail")
        {
            notificatie = BusinessLayer.Notificatie.Mail;
        }
        else if (notTekst == "Sms")
        {
            notificatie = BusinessLayer.Notificatie.Sms;
        }
        else if (notTekst == "Beide")
        {
            notificatie = BusinessLayer.Notificatie.Beide;
        }

        // Velden bijwerken op het Patient-object.
        _patient.Voornaam      = voornaam;
        _patient.Achternaam    = achternaam;
        _patient.Email         = email;
        _patient.Gsm           = gsm.Length > 0 ? gsm : null;
        _patient.Geslacht      = geslacht;
        _patient.Geboortedatum = CalGeboortedatum.SelectedDate.Value;
        _patient.Notificaties  = notificatie;

        // Nieuwe foto overnemen als de gebruiker er een heeft gekozen.
        if (_nieuweFotoBytes != null)
        {
            _patient.Profielfotodata = _nieuweFotoBytes;
        }

        try
        {
            _patient.Bijwerken();
        }
        catch (Exception ex)
        {
            TxtFoutBew.Text = "Fout bij opslaan: " + ex.Message;
            return;
        }

        // Topbalk van het hoofdvenster bijwerken met de nieuwe naam en foto.
        try
        {
            MainWindow mw = (MainWindow)Window.GetWindow(this);
            if (mw != null)
            {
                mw.RefreshTopbar(_patient);
            }
        }
        catch (Exception)
        {
            // Niet-kritiek: als het venster niet beschikbaar is, gewoon doorgaan.
        }

        ToonBekijkenPaneel();
    }

    /// <summary>
    /// Verwerpt de wijzigingen en gaat terug naar het bekijkpaneel.
    /// </summary>
    private void BtnAnnulerenBew_Click(object sender, RoutedEventArgs e)
    {
        ToonBekijkenPaneel();
    }

    // ── Hulpmethoden ──────────────────────────────────────────────────────────

    /// <summary>
    /// Vult de ComboBox met de vier notificatie-opties één keer bij aanmaken.
    /// </summary>
    private void VulNotificatiesComboBox()
    {
        CmbNotificaties.Items.Clear();
        CmbNotificaties.Items.Add("Geen");
        CmbNotificaties.Items.Add("Mail");
        CmbNotificaties.Items.Add("Sms");
        CmbNotificaties.Items.Add("Beide");
    }

    /// <summary>
    /// Laadt fotobytes in een Image-control; toont de placeholder als bytes null zijn
    /// of als het bestand niet geldig is.
    /// </summary>
    private void LaadFotoInControl(byte[] bytes, Image imgControl, Border placeholder)
    {
        if (bytes != null && bytes.Length > 0)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                MemoryStream ms    = new MemoryStream(bytes);
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.CacheOption  = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                imgControl.Source        = bitmap;
                imgControl.Visibility    = Visibility.Visible;
                placeholder.Visibility   = Visibility.Collapsed;
                return;
            }
            catch (Exception)
            {
                // Ongeldig afbeeldingsformaat: val door naar placeholder.
            }
        }

        imgControl.Source      = null;
        imgControl.Visibility  = Visibility.Collapsed;
        placeholder.Visibility = Visibility.Visible;
    }

    /// <summary>Vertaalt de enum-waarde naar een leesbare Nederlandse string.</summary>
    private string GeslachtNaarTekst(BusinessLayer.Geslacht g)
    {
        if (g == BusinessLayer.Geslacht.Man)              return "Man";
        if (g == BusinessLayer.Geslacht.Vrouw)            return "Vrouw";
        if (g == BusinessLayer.Geslacht.NietVanToepassing) return "Niet van toepassing";
        return "Onbekend";
    }

    /// <summary>Vertaalt de notificatie-enum naar een leesbare Nederlandse string.</summary>
    private string NotificatieNaarTekst(BusinessLayer.Notificatie n)
    {
        if (n == BusinessLayer.Notificatie.Mail)  return "Mail";
        if (n == BusinessLayer.Notificatie.Sms)   return "Sms";
        if (n == BusinessLayer.Notificatie.Beide) return "Beide";
        return "Geen";
    }
}
