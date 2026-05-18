using System.Windows.Controls;
using BusinessLayer;

namespace PatientApp;

/// <summary>
/// Profielpagina voor de patiënt — toont en bewerkt de persoonlijke gegevens.
/// (Inhoud wordt in een volgende stap uitgewerkt.)
/// </summary>
public partial class ProfielPage : Page
{
    // Ingelogde patiënt, doorgegeven via constructor (geen static, geen binding).
    private Patient _patient;

    /// <summary>
    /// Ontvangt de ingelogde patiënt en toont diens naam in de paginatitel.
    /// </summary>
    public ProfielPage(Patient patient)
    {
        InitializeComponent();
        _patient = patient;
        TxtTitel.Text = "Profiel van " + _patient.Voornaam + " " + _patient.Achternaam;
    }
}
