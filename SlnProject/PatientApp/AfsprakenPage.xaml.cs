using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BusinessLayer;

namespace PatientApp;

/// <summary>
/// Afsprakenpagina met twee panelen: overzicht en nieuwe afspraak aanmaken.
/// De ingelogde patiënt wordt via de constructor doorgegeven.
/// </summary>
public partial class AfsprakenPage : Page
{
    // Ingelogde patiënt; nooit null wanneer de pagina getoond wordt.
    private Patient _patient;

    // Spiegelt de zichtbare ListBox-items zodat we het bijbehorende
    // Afspraak-object kunnen ophalen zonder index-fouten.
    private List<Afspraak> _zichtbareAfspraken = new List<Afspraak>();

    // Beschikbare tijdvakken voor een nieuwe afspraak (elk uur van 9 t/m 17).
    private List<string> _tijdvakken = new List<string>();

    // Dokterslijst gespiegeld naast LstDokters.
    private List<Dokter> _dokters = new List<Dokter>();

    /// <summary>
    /// Ontvangt de ingelogde patiënt en laadt direct het overzicht.
    /// </summary>
    public AfsprakenPage(Patient patient)
    {
        InitializeComponent();
        _patient = patient;

        // Vul de tijdvakken-lijst één keer — wordt later gebruikt bij "Nieuwe afspraak".
        for (int uur = 9; uur <= 17; uur++)
        {
            _tijdvakken.Add(uur.ToString("D2") + ":00");
        }

        LaadAfspraken();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  PANEEL 1 — OVERZICHT
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Haalt de afspraken van de patiënt op en vult de ListBox.
    /// Past de "enkel toekomstige" filter toe wanneer die RadioButton actief is.
    /// </summary>
    private void LaadAfspraken()
    {
        LstAfspraken.Items.Clear();
        _zichtbareAfspraken.Clear();
        TxtKlacht.Text         = "";
        BtnAnnuleren.IsEnabled = false;
        TxtFout.Text           = "";

        List<Afspraak> alle = new List<Afspraak>();
        try
        {
            alle = Afspraak.GetByPatient(_patient.Id);
        }
        catch (Exception ex)
        {
            TxtFout.Text = "Fout bij ophalen afspraken: " + ex.Message;
            return;
        }

        // RbToekomstige kan null zijn vóór InitializeComponent klaar is.
        bool alleenToekomstig = RbToekomstige != null && RbToekomstige.IsChecked == true;

        foreach (Afspraak afspraak in alle)
        {
            if (alleenToekomstig && afspraak.Moment <= DateTime.Now)
            {
                continue;
            }

            // Doktersnaam ophalen voor de labelweergave.
            Dokter dokter = null;
            try
            {
                dokter = Dokter.GetById(afspraak.DokterId);
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij ophalen doktergegevens: " + ex.Message;
                return;
            }

            string dokterLabel = dokter != null
                ? "Dr. " + dokter.Voornaam[0] + ". " + dokter.Achternaam
                : "Onbekende dokter";

            // Formaat: "01/08/2026 om 9:15 - Dr. B. Bibber"
            string label = afspraak.Moment.ToString("dd/MM/yyyy") +
                           " om " + afspraak.Moment.ToString("H:mm") +
                           " - " + dokterLabel;

            LstAfspraken.Items.Add(label);
            _zichtbareAfspraken.Add(afspraak);
        }
    }

    /// <summary>
    /// Herlaadt de lijst wanneer de gebruiker van filterkeuze wisselt.
    /// </summary>
    private void RbFilter_Checked(object sender, RoutedEventArgs e)
    {
        // Kan vóór InitializeComponent worden aangeroepen; controleer beide velden.
        if (LstAfspraken == null || _patient == null) return;
        LaadAfspraken();
    }

    /// <summary>
    /// Toont de klacht en bepaalt of de annuleringsknop actief mag zijn.
    /// </summary>
    private void LstAfspraken_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int idx = LstAfspraken.SelectedIndex;
        if (idx < 0 || idx >= _zichtbareAfspraken.Count)
        {
            TxtKlacht.Text         = "";
            BtnAnnuleren.IsEnabled = false;
            return;
        }

        Afspraak gekozen = _zichtbareAfspraken[idx];
        TxtKlacht.Text = gekozen.Klacht;

        // Alleen toekomstige afspraken mogen worden geannuleerd.
        BtnAnnuleren.IsEnabled = gekozen.Moment > DateTime.Now;
    }

    /// <summary>
    /// Annuleert (verwijdert) de geselecteerde afspraak en herlaadt de lijst.
    /// </summary>
    private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
    {
        TxtFout.Text = "";

        int idx = LstAfspraken.SelectedIndex;
        if (idx < 0 || idx >= _zichtbareAfspraken.Count) return;

        Afspraak teAnnuleren = _zichtbareAfspraken[idx];

        // Dubbele controle: afspraak mag niet in het verleden liggen.
        if (teAnnuleren.Moment <= DateTime.Now)
        {
            TxtFout.Text = "Alleen toekomstige afspraken kunnen worden geannuleerd.";
            BtnAnnuleren.IsEnabled = false;
            return;
        }

        try
        {
            teAnnuleren.Annuleren();
        }
        catch (Exception ex)
        {
            TxtFout.Text = "Fout bij annuleren: " + ex.Message;
            return;
        }

        LaadAfspraken();
    }

    /// <summary>
    /// Schakelt naar het paneel voor een nieuwe afspraak.
    /// </summary>
    private void BtnNieuweAfspraak_Click(object sender, RoutedEventArgs e)
    {
        TxtFout.Text = "";
        ToonNieuweAfspraakPaneel();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  PANEEL 2 — NIEUWE AFSPRAAK
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Toont het paneel voor een nieuwe afspraak en vult de dokterslijst.
    /// </summary>
    private void ToonNieuweAfspraakPaneel()
    {
        PanelOverzicht.Visibility      = Visibility.Collapsed;
        PanelNieuweAfspraak.Visibility = Visibility.Visible;

        // Reset invoervelden.
        TxtFoutNieuw.Text        = "";
        TxtReden.Text            = "";
        CalDatum.SelectedDate    = null;
        PanelDokterInfo.Visibility = Visibility.Collapsed;

        // Vul tijdvakken in ComboBox.
        CmbTijd.Items.Clear();
        foreach (string tijdvak in _tijdvakken)
        {
            CmbTijd.Items.Add(tijdvak);
        }

        // Laad dokterslijst.
        LstDokters.Items.Clear();
        _dokters.Clear();

        List<Dokter> alleDoktersen = new List<Dokter>();
        try
        {
            alleDoktersen = Dokter.GetAll();
        }
        catch (Exception ex)
        {
            TxtFoutNieuw.Text = "Fout bij ophalen dokterslijst: " + ex.Message;
            return;
        }

        foreach (Dokter d in alleDoktersen)
        {
            LstDokters.Items.Add("Dr. " + d.Voornaam + " " + d.Achternaam);
            _dokters.Add(d);
        }
    }

    /// <summary>
    /// Toont de basisgegevens van de geselecteerde dokter.
    /// </summary>
    private void LstDokters_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int idx = LstDokters.SelectedIndex;
        if (idx < 0 || idx >= _dokters.Count)
        {
            PanelDokterInfo.Visibility = Visibility.Collapsed;
            return;
        }

        Dokter dokter = _dokters[idx];

        TxtDokterNaam.Text  = "Dr. " + dokter.Voornaam + " " + dokter.Achternaam;
        TxtDokterEmail.Text = "E-mail: " + dokter.Email;
        TxtDokterGsm.Text   = "GSM: " + (string.IsNullOrEmpty(dokter.Gsm) ? "—" : dokter.Gsm);
        TxtDokterRiziv.Text = "RIZIV-nummer: " + dokter.Rizivnummer;
        TxtDokterConv.Text  = "Geconventioneerd: " + (dokter.IsGeconventioneerd ? "Ja" : "Nee");

        PanelDokterInfo.Visibility = Visibility.Visible;
    }

    /// <summary>
    /// Verplicht geselecteerde datum in de toekomst; geeft anders een melding.
    /// </summary>
    private void CalDatum_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        TxtFoutNieuw.Text = "";

        if (CalDatum.SelectedDate.HasValue && CalDatum.SelectedDate.Value.Date < DateTime.Now.Date)
        {
            TxtFoutNieuw.Text      = "Kies een datum in de toekomst.";
            CalDatum.SelectedDate  = null;
        }
    }

    /// <summary>
    /// Valideert de invoer en slaat de nieuwe afspraak op.
    /// </summary>
    private void BtnBevestigen_Click(object sender, RoutedEventArgs e)
    {
        TxtFoutNieuw.Text = "";

        // Validatie: alle velden verplicht.
        if (LstDokters.SelectedIndex < 0)
        {
            TxtFoutNieuw.Text = "Kies een dokter.";
            return;
        }

        if (!CalDatum.SelectedDate.HasValue)
        {
            TxtFoutNieuw.Text = "Kies een datum.";
            return;
        }

        if (CmbTijd.SelectedIndex < 0)
        {
            TxtFoutNieuw.Text = "Kies een tijdstip.";
            return;
        }

        string reden = TxtReden.Text.Trim();
        if (reden.Length == 0)
        {
            TxtFoutNieuw.Text = "Geef een reden of klacht op.";
            return;
        }

        // Datum + tijdstip samenvoegen tot één DateTime.
        DateTime gekozenDatum = CalDatum.SelectedDate.Value.Date;
        string tijdTekst      = _tijdvakken[CmbTijd.SelectedIndex]; // bijv. "09:00"

        // Uren en minuten extraheren zonder TryParse (VERBODEN wegens out-parameter).
        int uur    = 0;
        int minuut = 0;
        try
        {
            string[] delen = tijdTekst.Split(':');
            uur    = Convert.ToInt32(delen[0]);
            minuut = Convert.ToInt32(delen[1]);
        }
        catch (Exception)
        {
            TxtFoutNieuw.Text = "Ongeldig tijdstip geselecteerd.";
            return;
        }

        DateTime moment = new DateTime(
            gekozenDatum.Year, gekozenDatum.Month, gekozenDatum.Day,
            uur, minuut, 0);

        if (moment <= DateTime.Now)
        {
            TxtFoutNieuw.Text = "Kies een datum en tijdstip in de toekomst.";
            return;
        }

        Dokter gekozenDokter = _dokters[LstDokters.SelectedIndex];

        Afspraak nieuw = new Afspraak();
        nieuw.Moment    = moment;
        nieuw.Klacht    = reden;
        nieuw.PatientId = _patient.Id;
        nieuw.DokterId  = gekozenDokter.Id;

        try
        {
            nieuw.Opslaan();
        }
        catch (Exception ex)
        {
            TxtFoutNieuw.Text = "Fout bij opslaan afspraak: " + ex.Message;
            return;
        }

        // Succes: terug naar overzicht.
        ToonOverzichtPaneel();
    }

    /// <summary>
    /// Gaat terug naar het overzicht zonder op te slaan.
    /// </summary>
    private void BtnTerug_Click(object sender, RoutedEventArgs e)
    {
        ToonOverzichtPaneel();
    }

    // ── Hulpmethoden ──────────────────────────────────────────────────────────

    /// <summary>Toont het overzichtspaneel en herlaadt de lijst.</summary>
    private void ToonOverzichtPaneel()
    {
        PanelNieuweAfspraak.Visibility = Visibility.Collapsed;
        PanelOverzicht.Visibility      = Visibility.Visible;
        LaadAfspraken();
    }
}
