using System;
using System.Windows;
using System.Windows.Controls;
using BusinessLayer;

namespace DokterApp;

/// <summary>
/// Loginpagina voor de artsendashboard-app.
/// Valideert het formulier, zoekt de dokter op en controleert het wachtwoord.
/// Bij succes koppelt het terug naar MainWindow via VerwerkInloggen().
/// </summary>
public partial class LoginPage : Page
{
    // Referentie naar het hoofdvenster, nodig om na inloggen de topbalk bij te werken.
    private MainWindow _hoofdvenster;

    /// <summary>
    /// Ontvangt het hoofdvenster zodat LoginPage na succesvolle login
    /// de naam/foto en menuknoppen in MainWindow kan laten bijwerken.
    /// </summary>
    public LoginPage(MainWindow hoofdvenster)
    {
        InitializeComponent();
        _hoofdvenster = hoofdvenster;

        // Voorinvullen met het testaccount maakt beoordeling eenvoudiger.
        TxtEmail.Text        = "alexandre@dokterspraktijk.be";
        PbxWachtwoord.Password = "t9ZmRrAbSfCv";
    }

    // ── Inlogknop ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Verwerkt een inlogpoging: validatie, database-opzoeking en wachtwoordcontrole.
    /// Fouten worden getoond in TxtFout — nooit via een MessageBox.
    /// </summary>
    private void BtnInloggen_Click(object sender, RoutedEventArgs e)
    {
        // Reset eventuele vorige foutmelding.
        VerbergFout();

        string email     = TxtEmail.Text.Trim();
        string wachtwoord = PbxWachtwoord.Password;

        // ── Formuliervalidatie ─────────────────────────────────────────────────

        // Beide velden moeten ingevuld zijn.
        if (email.Length == 0 || wachtwoord.Length == 0)
        {
            ToonFout("Vul zowel uw e-mailadres als uw wachtwoord in.");
            return;
        }

        // Basiscontrole op e-mailformaat: moet '@' en '.' bevatten.
        if (!email.Contains("@") || !email.Contains("."))
        {
            ToonFout("Voer een geldig e-mailadres in (bevat '@' en '.').");
            return;
        }

        // ── Database-opzoeking ─────────────────────────────────────────────────

        Dokter gevonden = null;
        try
        {
            // Zoek de dokter op via het e-mailadres; geeft null terug als niet gevonden.
            gevonden = Dokter.GetByEmail(email);
        }
        catch (Exception ex)
        {
            ToonFout("Databasefout bij het ophalen van het account: " + ex.Message);
            return;
        }

        // Null = e-mailadres bestaat niet in de DB.
        if (gevonden == null)
        {
            // Generieke melding om gebruikersoptelbaar te blijven (geen hint of het e-mail of
            // het wachtwoord fout is).
            ToonFout("E-mailadres of wachtwoord onjuist.");
            return;
        }

        // ── Wachtwoordcontrole ─────────────────────────────────────────────────

        bool paswoordKlopt = false;
        try
        {
            // ControleerPaswoord hashed de invoer en vergelijkt met de opgeslagen hash.
            paswoordKlopt = gevonden.ControleerPaswoord(wachtwoord);
        }
        catch (Exception ex)
        {
            ToonFout("Fout bij het controleren van het wachtwoord: " + ex.Message);
            return;
        }

        if (!paswoordKlopt)
        {
            ToonFout("E-mailadres of wachtwoord onjuist.");
            return;
        }

        // ── Inloggen geslaagd ──────────────────────────────────────────────────

        // Geef de ingelogde dokter door aan het hoofdvenster; dat zorgt voor
        // de naam/foto in de topbalk, het activeren van de knoppen en de navigatie.
        _hoofdvenster.VerwerkInloggen(gevonden);
    }

    // ── Hulpmethoden ──────────────────────────────────────────────────────────

    /// <summary>Toont een foutmelding in de TextBlock op de pagina.</summary>
    private void ToonFout(string bericht)
    {
        TxtFout.Text       = bericht;
        TxtFout.Visibility = Visibility.Visible;
    }

    /// <summary>Verbergt de foutmelding TextBlock.</summary>
    private void VerbergFout()
    {
        TxtFout.Text       = "";
        TxtFout.Visibility = Visibility.Collapsed;
    }
}
