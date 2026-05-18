using System;
using System.Windows;
using System.Windows.Controls;
using BusinessLayer;

namespace DokterApp;

/// <summary>
/// Bevestigingspagina voor het verwijderen van een patiënt.
/// De bevestiging verloopt volledig in-page via twee knoppen (Ja / Nee) —
/// nooit via een MessageBox.
/// Bij "Ja" worden de patiënt en alle gekoppelde afspraken verwijderd
/// via patient.Verwijderen() (dat verwijdert ook de Afspraak-rijen).
/// </summary>
public partial class PatientVerwijderenPage : Page
{
    // Referentie naar de shell-pagina voor navigatie.
    private PatientenPage _parent;

    // De te verwijderen patiënt.
    private Patient _patient;

    // Ingelogde dokter — meegestuurd bij terugnavigatie.
    private Dokter _dokter;

    /// <summary>
    /// Ontvangt de patiënt en toont diens naam in de bevestigingstekst.
    /// </summary>
    public PatientVerwijderenPage(PatientenPage parent, Patient patient, Dokter dokter)
    {
        InitializeComponent();
        _parent  = parent;
        _patient = patient;
        _dokter  = dokter;

        TxtPatientNaam.Text = "Patiënt: " + _patient.Voornaam + " " + _patient.Achternaam;
    }

    // ── Ja-knop ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Verwijdert de patiënt en alle gekoppelde afspraken uit de database,
    /// daarna navigeert het terug naar het patiëntenoverzicht.
    /// Bij een fout wordt een melding getoond in TxtFout — nooit via een MessageBox.
    /// </summary>
    private void BtnJa_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // patient.Verwijderen() verwijdert eerst de Afspraak-rijen en dan de patiënt zelf.
            _patient.Verwijderen();
        }
        catch (Exception ex)
        {
            TxtFout.Text       = "Fout bij verwijderen: " + ex.Message;
            TxtFout.Visibility = Visibility.Visible;
            return;
        }

        // Terug naar het overzicht na succesvolle verwijdering.
        _parent.NavigeerNaar(new PatientenOverzichtPage(_parent, _dokter));
    }

    // ── Nee-knop ──────────────────────────────────────────────────────────────

    /// <summary>Annuleert de verwijdering en gaat terug naar het overzicht.</summary>
    private void BtnNee_Click(object sender, RoutedEventArgs e)
    {
        _parent.NavigeerNaar(new PatientenOverzichtPage(_parent, _dokter));
    }

    // ── Terug-knop ────────────────────────────────────────────────────────────

    /// <summary>Gaat terug naar het overzicht zonder te verwijderen.</summary>
    private void BtnTerug_Click(object sender, RoutedEventArgs e)
    {
        _parent.NavigeerNaar(new PatientenOverzichtPage(_parent, _dokter));
    }
}
