using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BusinessLayer;

namespace DokterApp;

/// <summary>
/// Detailpagina van een patiënt — alle velden worden read-only getoond via TextBlocks.
/// Bevat een "Terug naar overzicht"-knop die navigeert naar PatientenOverzichtPage.
/// </summary>
public partial class PatientDetailsPage : Page
{
    // Referentie naar de shell-pagina voor navigatie.
    private PatientenPage _parent;

    // De te tonen patiënt.
    private Patient _patient;

    // Ingelogde dokter — meegestuurd bij terugnavigatie.
    private Dokter _dokter;

    /// <summary>
    /// Ontvangt de patiënt en vult alle velden in vanuit code-behind (geen binding).
    /// </summary>
    public PatientDetailsPage(PatientenPage parent, Patient patient, Dokter dokter)
    {
        InitializeComponent();
        _parent  = parent;
        _patient = patient;
        _dokter  = dokter;
        VulVelden();
    }

    // ── Velden invullen ───────────────────────────────────────────────────────

    /// <summary>Vult alle TextBlocks en laadt de profielfoto via een MemoryStream.</summary>
    private void VulVelden()
    {
        TxtTitel.Text       = _patient.Voornaam + " " + _patient.Achternaam;
        TxtVoornaam.Text    = _patient.Voornaam;
        TxtAchternaam.Text  = _patient.Achternaam;
        TxtGeslacht.Text    = GeslachtNaarTekst(_patient.Geslacht);
        TxtGeboortedatum.Text = _patient.Geboortedatum.ToString("dd/MM/yyyy");
        TxtEmail.Text       = _patient.Email;
        TxtGsm.Text         = _patient.Gsm ?? "—";
        TxtNotificaties.Text = NotificatieNaarTekst(_patient.Notificaties);

        // Profielfoto laden — bij fout blijft de placeholder zichtbaar.
        if (_patient.Profielfotodata != null && _patient.Profielfotodata.Length > 0)
        {
            try
            {
                BitmapImage bmp = new BitmapImage();
                MemoryStream ms  = new MemoryStream(_patient.Profielfotodata);
                bmp.BeginInit();
                bmp.StreamSource = ms;
                bmp.CacheOption  = BitmapCacheOption.OnLoad;
                bmp.EndInit();

                ImgFoto.Source                    = bmp;
                ImgFoto.Visibility                = Visibility.Visible;
                BorderPlaceholderDetail.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                // Ongeldig formaat: de placeholder blijft zichtbaar.
            }
        }
    }

    // ── Navigatie ─────────────────────────────────────────────────────────────

    /// <summary>Navigeert terug naar het patiëntenoverzicht.</summary>
    private void BtnTerug_Click(object sender, RoutedEventArgs e)
    {
        _parent.NavigeerNaar(new PatientenOverzichtPage(_parent, _dokter));
    }

    // ── Hulpmethoden ──────────────────────────────────────────────────────────

    /// <summary>Zet de Geslacht-enum om naar een leesbare Nederlandse tekst.</summary>
    private string GeslachtNaarTekst(Geslacht geslacht)
    {
        if (geslacht == Geslacht.Man)               return "Man";
        if (geslacht == Geslacht.Vrouw)             return "Vrouw";
        if (geslacht == Geslacht.NietVanToepassing) return "Niet van toepassing";
        return "Onbekend";
    }

    /// <summary>Zet de Notificatie-enum om naar een leesbare Nederlandse tekst.</summary>
    private string NotificatieNaarTekst(Notificatie notificatie)
    {
        if (notificatie == Notificatie.Mail)  return "Mail";
        if (notificatie == Notificatie.Sms)   return "Sms";
        if (notificatie == Notificatie.Beide) return "Mail en sms";
        return "Geen";
    }
}
