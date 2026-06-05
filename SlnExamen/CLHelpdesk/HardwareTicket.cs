using System.Text;

namespace CLHelpdesk
{
    // Subklasse van Ticket voor hardwareproblemen (printers, laptops, monitoren, …).
    // Voegt de eigenschap Toestel toe en breidt GeefInfo() uit met
    // het type en de toestelomschrijving.
    public class HardwareTicket : Ticket
    {
        // Omschrijving van het betrokken apparaat (bijv. "Dell Latitude 5420").
        public string Toestel { get; set; } = string.Empty;

        // Voegt "Type: Hardware" en "Toestel: ..." toe aan de basisinformatie.
        // De afgesloten-regel wordt al door de basisklasse afgehandeld.
        public override string GeefInfo()
        {
            StringBuilder sb = new StringBuilder(base.GeefInfo());

            sb.AppendLine("Type:        Hardware");
            sb.AppendLine($"Toestel:     {Toestel}");

            return sb.ToString();
        }
    }
}
