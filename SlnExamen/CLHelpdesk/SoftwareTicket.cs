using System.Text;

namespace CLHelpdesk
{
    // Subklasse van Ticket voor softwareproblemen (applicaties, browsers, clouddiensten, …).
    // Voegt de eigenschap Applicatie toe en breidt GeefInfo() uit met
    // het type en de applicatienaam.
    public class SoftwareTicket : Ticket
    {
        // Naam van de betrokken applicatie (bijv. "Microsoft Teams").
        public string Applicatie { get; set; } = string.Empty;

        // Voegt "Type: Software" en "Applicatie: ..." toe aan de basisinformatie.
        public override string GeefInfo()
        {
            StringBuilder sb = new StringBuilder(base.GeefInfo());

            sb.AppendLine("Type:        Software");
            sb.AppendLine($"Applicatie:  {Applicatie}");

            return sb.ToString();
        }
    }
}
