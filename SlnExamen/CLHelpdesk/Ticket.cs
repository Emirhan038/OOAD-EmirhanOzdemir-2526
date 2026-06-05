using System;
using System.Text;

namespace CLHelpdesk
{
    // Basisklasse voor alle helpdesktickets.
    // Bevat de gemeenschappelijke eigenschappen en de virtuele GeefInfo()-methode
    // die door subklassen (HardwareTicket, SoftwareTicket) overschreven wordt.
    public class Ticket
    {
        // Uniek volgnummer van het ticket in het systeem.
        public int Id { get; set; }

        // Korte omschrijving van het probleem.
        public string Titel { get; set; } = string.Empty;

        // Voornaam van de persoon die het ticket heeft ingediend.
        public string MelderVoornaam { get; set; } = string.Empty;

        // Achternaam van de persoon die het ticket heeft ingediend.
        public string MelderAchternaam { get; set; } = string.Empty;

        // Gebruikersnaam (login-id) van de melder.
        public string MelderId { get; set; } = string.Empty;

        // Urgentieniveau: Laag, Normaal of Hoog.
        public Prioriteit Prioriteit { get; set; }

        // Geeft aan of het ticket al dan niet is afgehandeld.
        public bool IsAfgesloten { get; set; }

        // Tijdstip waarop het ticket werd aangemaakt.
        public DateTime DatumAangemaakt { get; set; }

        // Tijdstip waarop het ticket werd afgesloten; null zolang het nog open is.
        public DateTime? DatumAfgesloten { get; set; }

        // Geeft de basisinformatie van het ticket terug als opgemaakte tekst.
        // Subklassen voegen hieraan hun typespecifieke regels toe via override.
        public virtual string GeefInfo()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Id:          {Id}");
            sb.AppendLine($"Titel:       {Titel}");
            sb.AppendLine($"Melder:      {MelderVoornaam} {MelderAchternaam} ({MelderId})");
            sb.AppendLine($"Prioriteit:  {Prioriteit}");
            sb.AppendLine($"Aangemaakt:  {DatumAangemaakt:dd/MM/yyyy HH:mm}");

            // Afgesloten-regel verschijnt alleen wanneer het ticket daadwerkelijk is afgesloten.
            if (IsAfgesloten && DatumAfgesloten.HasValue)
                sb.AppendLine($"Afgesloten:  {DatumAfgesloten.Value:dd/MM/yyyy HH:mm}");

            return sb.ToString();
        }
    }
}
