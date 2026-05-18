using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BusinessLayer;

namespace DokterApp;

/// <summary>
/// Afsprakenpagina voor de arts.
/// Toont een kalender links en de afspraken van de geselecteerde dag rechts.
/// De arts kan een afspraak selecteren om de reden te lezen en de afspraak te annuleren.
/// </summary>
public partial class AfsprakenPage : Page
{
    // Ingelogde dokter, doorgegeven via constructor.
    private Dokter _dokter;

    // Alle afspraken van deze dokter, eenmalig geladen bij het openen van de pagina.
    private List<Afspraak> _alleAfspraken = new List<Afspraak>();

    // Afspraken voor de geselecteerde kalenderdag — subset van _alleAfspraken.
    private List<Afspraak> _afsprakenVanDag = new List<Afspraak>();

    // Alle patiënten, eenmalig geladen voor het opzoeken van namen bij afspraken.
    private List<Patient> _allePatienten = new List<Patient>();

    /// <summary>
    /// Ontvangt de ingelogde dokter, toont zijn naam en laadt initiële gegevens.
    /// </summary>
    public AfsprakenPage(Dokter dokter)
    {
        InitializeComponent();
        _dokter = dokter;
        TxtTitel.Text = "Afspraken van dr. " + _dokter.Voornaam + " " + _dokter.Achternaam;
        LaadGegevens();

        // Selecteer vandaag zodat de lijst meteen zichtbaar is na inloggen.
        KalenderAfspraken.SelectedDate = DateTime.Today;
    }

    // ── Gegevens laden ────────────────────────────────────────────────────────

    /// <summary>Laadt alle afspraken en patiënten eenmalig uit de database.</summary>
    private void LaadGegevens()
    {
        try
        {
            _alleAfspraken = Afspraak.GetByDokter(_dokter.Id);
        }
        catch (Exception ex)
        {
            ToonFout("Fout bij ophalen afspraken: " + ex.Message);
        }

        try
        {
            _allePatienten = Patient.GetAll();
        }
        catch (Exception ex)
        {
            ToonFout("Fout bij ophalen patiënten: " + ex.Message);
        }
    }

    /// <summary>
    /// Herlaadt alle afspraken uit de database — aangeroepen na een annulering.
    /// </summary>
    private void HerlaadAfspraken()
    {
        _alleAfspraken = new List<Afspraak>();
        try
        {
            _alleAfspraken = Afspraak.GetByDokter(_dokter.Id);
        }
        catch (Exception ex)
        {
            ToonFout("Fout bij vernieuwen afspraken: " + ex.Message);
        }
    }

    // ── Kalender ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Wordt aangeroepen wanneer de gebruiker een datum selecteert op de kalender.
    /// Filtert de afspraken van die dag en toont ze in de ListBox.
    /// </summary>
    private void KalenderAfspraken_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        VerbergFout();
        VerbergReden();
        BtnAnnuleren.IsEnabled = false;

        if (KalenderAfspraken.SelectedDate == null)
        {
            LstAfspraken.Items.Clear();
            _afsprakenVanDag = new List<Afspraak>();
            TxtDatumLabel.Text = "Kies een datum om afspraken te zien.";
            return;
        }

        DateTime datum = KalenderAfspraken.SelectedDate.Value.Date;
        TxtDatumLabel.Text = "Afspraken op " + datum.ToString("dd/MM/yyyy") + ":";

        // Filter zonder LINQ: foreach over alle afspraken, vergelijk de datumcomponent.
        _afsprakenVanDag = new List<Afspraak>();
        foreach (Afspraak afspraak in _alleAfspraken)
        {
            if (afspraak.Moment.Date == datum)
            {
                _afsprakenVanDag.Add(afspraak);
            }
        }

        VulListBox();
    }

    // ── ListBox ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Vult de ListBox met de afspraken van de geselecteerde dag.
    /// Formaat: "HH:mm - Voornaam Achternaam".
    /// </summary>
    private void VulListBox()
    {
        LstAfspraken.Items.Clear();
        foreach (Afspraak afspraak in _afsprakenVanDag)
        {
            string naam = ZoekPatientNaam(afspraak.PatientId);
            LstAfspraken.Items.Add(afspraak.Moment.ToString("HH:mm") + " – " + naam);
        }
    }

    /// <summary>
    /// Zoekt de volledige naam van een patiënt op basis van zijn id.
    /// Gebruikt foreach (geen LINQ); geeft "(onbekende patiënt)" als niet gevonden.
    /// </summary>
    private string ZoekPatientNaam(int patientId)
    {
        foreach (Patient patient in _allePatienten)
        {
            if (patient.Id == patientId)
            {
                return patient.Voornaam + " " + patient.Achternaam;
            }
        }
        return "(onbekende patiënt)";
    }

    /// <summary>
    /// Toont de reden van consultatie wanneer een afspraak geselecteerd wordt.
    /// Schakelt ook de annuleer-knop in.
    /// </summary>
    private void LstAfspraken_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int index = LstAfspraken.SelectedIndex;
        if (index < 0 || index >= _afsprakenVanDag.Count)
        {
            VerbergReden();
            BtnAnnuleren.IsEnabled = false;
            return;
        }

        Afspraak geselecteerd = _afsprakenVanDag[index];
        TxtReden.Text = "Reden van consultatie: " + geselecteerd.Klacht;
        BorderReden.Visibility = Visibility.Visible;
        BtnAnnuleren.IsEnabled = true;
    }

    // ── Annuleer-knop ─────────────────────────────────────────────────────────

    /// <summary>
    /// Verwijdert de geselecteerde afspraak uit de database en vernieuwt de lijst.
    /// Bij een fout wordt een melding getoond in TxtFout — nooit via een MessageBox.
    /// </summary>
    private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
    {
        int index = LstAfspraken.SelectedIndex;
        if (index < 0 || index >= _afsprakenVanDag.Count)
        {
            return;
        }

        Afspraak teAnnuleren = _afsprakenVanDag[index];
        try
        {
            teAnnuleren.Annuleren();
        }
        catch (Exception ex)
        {
            ToonFout("Fout bij annuleren van de afspraak: " + ex.Message);
            return;
        }

        // Herlaad de lijst vanuit de database en ververs de weergave.
        HerlaadAfspraken();
        VerbergReden();
        BtnAnnuleren.IsEnabled = false;

        // Herfilter voor de huidige geselecteerde dag.
        if (KalenderAfspraken.SelectedDate != null)
        {
            DateTime datum = KalenderAfspraken.SelectedDate.Value.Date;
            _afsprakenVanDag = new List<Afspraak>();
            foreach (Afspraak a in _alleAfspraken)
            {
                if (a.Moment.Date == datum)
                {
                    _afsprakenVanDag.Add(a);
                }
            }
        }
        else
        {
            _afsprakenVanDag = new List<Afspraak>();
        }

        VulListBox();
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

    /// <summary>Verbergt het redenblok.</summary>
    private void VerbergReden()
    {
        TxtReden.Text            = "";
        BorderReden.Visibility   = Visibility.Collapsed;
    }
}
