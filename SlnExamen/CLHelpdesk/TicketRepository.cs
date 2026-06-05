using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace CLHelpdesk
{
    // Verantwoordelijk voor het inlezen van tickets uit het CSV-bestand.
    // Elke rij in de CSV wordt omgezet naar een HardwareTicket of SoftwareTicket
    // op basis van de kolom 'type'.
    //
    // Verwacht kolomvolgorde (puntkomma als scheidingsteken, eerste rij is header):
    //   id ; titel ; melderVoornaam ; melderAchternaam ; melderId ;
    //   prioriteit ; isAfgesloten ; type ; extraInfo ;
    //   datumAangemaakt ; datumAfgesloten
    public static class TicketRepository
    {
        // Datumnotatie zoals aanwezig in het CSV-bestand (bijv. "2026-04-20 0915").
        private const string DatumFormaat = "yyyy-MM-dd HHmm";

        // Leest alle tickets uit het opgegeven CSV-bestand en geeft ze terug als lijst.
        // Onleesbare regels worden overgeslagen (geen crash bij fout in één rij).
        public static List<Ticket> LaadUitCsv(string bestandspad)
        {
            List<Ticket> tickets = new List<Ticket>();

            // Alle regels inlezen; de eerste rij (header) wordt overgeslagen.
            string[] regels = File.ReadAllLines(bestandspad);

            for (int i = 1; i < regels.Length; i++)
            {
                // Aanhalingstekens rondom de volledige rij verwijderen (CSV-stijl).
                string rij = regels[i].Trim().Trim('"');

                if (string.IsNullOrEmpty(rij))
                    continue;

                // Kolommen splitsen op puntkomma.
                string[] kolommen = rij.Split(';');

                // Minimaal 10 kolommen vereist (datumAfgesloten mag leeg zijn).
                if (kolommen.Length < 10)
                    continue;

                try
                {
                    // ── Gemeenschappelijke velden uitlezen ────────────────────
                    int       id              = int.Parse(kolommen[0].Trim());
                    string    titel           = kolommen[1].Trim();
                    string    melderVoornaam  = kolommen[2].Trim();
                    string    melderAchternaam = kolommen[3].Trim();
                    string    melderId        = kolommen[4].Trim();
                    Prioriteit prioriteit     = ParsePrioriteit(kolommen[5].Trim());
                    bool      isAfgesloten    = bool.Parse(kolommen[6].Trim());
                    string    type            = kolommen[7].Trim();
                    string    extraInfo       = kolommen[8].Trim();

                    DateTime aangemaakt = DateTime.ParseExact(
                        kolommen[9].Trim(), DatumFormaat, CultureInfo.InvariantCulture);

                    // DatumAfgesloten is optioneel: leeg wanneer het ticket nog open is.
                    DateTime? afgesloten = null;
                    if (kolommen.Length > 10 && !string.IsNullOrWhiteSpace(kolommen[10]))
                    {
                        afgesloten = DateTime.ParseExact(
                            kolommen[10].Trim(), DatumFormaat, CultureInfo.InvariantCulture);
                    }

                    // ── Juiste subklasse aanmaken op basis van het type ───────
                    Ticket ticket;

                    if (type.Equals("Hardware", StringComparison.OrdinalIgnoreCase))
                    {
                        ticket = new HardwareTicket { Toestel = extraInfo };
                    }
                    else
                    {
                        // Alles wat geen "Hardware" is, behandelen we als Software.
                        ticket = new SoftwareTicket { Applicatie = extraInfo };
                    }

                    // Gemeenschappelijke eigenschappen invullen.
                    ticket.Id               = id;
                    ticket.Titel            = titel;
                    ticket.MelderVoornaam   = melderVoornaam;
                    ticket.MelderAchternaam = melderAchternaam;
                    ticket.MelderId         = melderId;
                    ticket.Prioriteit       = prioriteit;
                    ticket.IsAfgesloten     = isAfgesloten;
                    ticket.DatumAangemaakt  = aangemaakt;
                    ticket.DatumAfgesloten  = afgesloten;

                    tickets.Add(ticket);
                }
                catch
                {
                    // Ongeldige rij overslaan zonder het inlezen te onderbreken.
                }
            }

            return tickets;
        }

        // Zet de tekstwaarde uit het CSV-bestand om naar het Prioriteit-enum.
        // Onbekende waarden krijgen standaard Normaal toegewezen.
        private static Prioriteit ParsePrioriteit(string waarde)
        {
            return waarde switch
            {
                "Laag"    => Prioriteit.Laag,
                "Normaal" => Prioriteit.Normaal,
                "Hoog"    => Prioriteit.Hoog,
                _         => Prioriteit.Normaal
            };
        }
    }
}
