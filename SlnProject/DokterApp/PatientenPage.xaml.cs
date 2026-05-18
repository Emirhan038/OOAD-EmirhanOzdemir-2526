using System.Windows.Controls;
using BusinessLayer;

namespace DokterApp;

/// <summary>
/// Shell-pagina voor het patiëntenbeheer.
/// Bevat één Frame (SubFrame) dat de vier subpagina's toont:
/// overzicht, details, toevoegen/wijzigen en verwijderen.
/// Navigatie loopt altijd via NavigeerNaar() zodat subpagina's
/// niet rechtstreeks aan het SubFrame hoeven te raken.
/// </summary>
public partial class PatientenPage : Page
{
    // Ingelogde dokter, doorgegeven via constructor en meegestuurd naar subpagina's.
    private Dokter _dokter;

    /// <summary>
    /// Ontvangt de ingelogde dokter en toont meteen het patiëntenoverzicht.
    /// </summary>
    public PatientenPage(Dokter dokter)
    {
        InitializeComponent();
        _dokter = dokter;
        NavigeerNaar(new PatientenOverzichtPage(this, _dokter));
    }

    /// <summary>
    /// Laadt de opgegeven subpagina in het interne Frame.
    /// Alle subpagina's roepen deze methode aan om te navigeren.
    /// </summary>
    public void NavigeerNaar(Page pagina)
    {
        SubFrame.Content = pagina;
    }
}
