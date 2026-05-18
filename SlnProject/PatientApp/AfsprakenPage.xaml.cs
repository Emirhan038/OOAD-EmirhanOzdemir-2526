using System.Windows.Controls;
using BusinessLayer;

namespace PatientApp;

/// <summary>
/// Afsprakenpagina voor de patiënt — toont een overzicht van geplande afspraken.
/// (Inhoud wordt in een volgende stap uitgewerkt.)
/// </summary>
public partial class AfsprakenPage : Page
{
    // Ingelogde patiënt, doorgegeven via constructor (geen static, geen binding).
    private Patient _patient;

    /// <summary>
    /// Ontvangt de ingelogde patiënt en toont diens naam in de paginatitel.
    /// </summary>
    public AfsprakenPage(Patient patient)
    {
        InitializeComponent();
        _patient = patient;
        TxtTitel.Text = "Afspraken van " + _patient.Voornaam + " " + _patient.Achternaam;
    }
}
