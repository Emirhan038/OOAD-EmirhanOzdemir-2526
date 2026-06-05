#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CLHelpdesk;

namespace WpfHelpdesk
{
    // Hoofdvenster van de helpdesktoepassing.
    // Alle interactie (filters, detail, nieuw ticket, afsluiten) zit in dit ene venster.
    // Geen LINQ, geen var, geen out/TryParse, geen MessageBox, geen DataGrid, geen Binding.
    public partial class MainWindow : Window
    {
        // Pad naar het CSV-bestand, relatief ten opzichte van de uitvoermap van de applicatie.
        private const string CsvPad = @"..\..\..\..\helpdesk_tickets.csv";

        // Volledige lijst van tickets zoals ingeladen uit het CSV-bestand.
        // Deze lijst blijft ongewijzigd; gefilterd wordt via PasFiltersTo().
        private List<Ticket> alleTickets = new List<Ticket>();

        // Absolute bestandspad dat bepaald wordt bij het opstarten.
        private string volledigCsvPad = string.Empty;

        // Volgnummer voor nieuwe tickets: hoogste bestaand id + 1.
        private int volgendId = 1;

        public MainWindow()
        {
            InitializeComponent();

            // ── 1. Prioriteit-filter vullen ─────────────────────────────────────────
            // Eerste item is altijd "Alle" (geen beperking op prioriteit).
            cmbPrioriteitFilter.Items.Add("Alle");

            // Elk enum-lid van Prioriteit toevoegen als tekstwaarde.
            foreach (Prioriteit p in Enum.GetValues(typeof(Prioriteit)))
            {
                cmbPrioriteitFilter.Items.Add(p.ToString());
            }

            // Standaard staat "Alle" geselecteerd.
            cmbPrioriteitFilter.SelectedIndex = 0;

            // ── 2. Prioriteit-ComboBox voor nieuw ticket vullen ──────────────────────
            // Dezelfde enum-waarden, maar hier geen "Alle"-optie.
            foreach (Prioriteit p in Enum.GetValues(typeof(Prioriteit)))
            {
                cmbPrioriteitNieuw.Items.Add(p.ToString());
            }
            cmbPrioriteitNieuw.SelectedIndex = 1; // Normaal als standaard

            // ── 3. Type-ComboBox: standaardselectie Hardware ─────────────────────────
            // De items zijn al in XAML gedefinieerd (Hardware / Software).
            cmbType.SelectedIndex = 0;

            // ── 4. Tickets inladen uit CSV ───────────────────────────────────────────
            volledigCsvPad = Path.GetFullPath(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CsvPad));

            LaadTicketsUitCsv();
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // INLADEN
        // ─────────────────────────────────────────────────────────────────────────────

        // Leest het CSV-bestand in via TicketRepository, vult de melder-ComboBoxen
        // en past de filters toe zodat de ListBox gevuld wordt.
        private void LaadTicketsUitCsv()
        {
            // Bestand controleren voordat we proberen te lezen.
            if (!File.Exists(volledigCsvPad))
            {
                ToonFout("CSV-bestand niet gevonden: " + volledigCsvPad);
                return;
            }

            alleTickets = TicketRepository.LaadUitCsv(volledigCsvPad);

            // Hoogste id bepalen voor automatische nummering van nieuwe tickets.
            volgendId = 1;
            foreach (Ticket t in alleTickets)
            {
                if (t.Id >= volgendId)
                    volgendId = t.Id + 1;
            }

            // Melder-ComboBoxen opnieuw opbouwen op basis van de geladen tickets.
            VulMelderComboBoxen();

            // Gefilterde weergave bijwerken.
            PasFiltersTo();
        }

        // Vult beide melder-ComboBoxen (filter + nieuw ticket) met unieke melder-namen.
        // De filter-ComboBox krijgt vooraf "Alle"; de nieuw-ticket-ComboBox niet.
        private void VulMelderComboBoxen()
        {
            // Tijdelijk de SelectionChanged-handler loskoppelen om cascade te vermijden.
            cmbMelderFilter.SelectionChanged -= FilterGewijzigd;

            cmbMelderFilter.Items.Clear();
            cmbMelderFilter.Items.Add("Alle");

            cmbMelderNieuw.Items.Clear();

            // Unieke melder-namen bijhouden via een gewone lijst (geen HashSet/LINQ).
            List<string> gezieneNamen = new List<string>();

            foreach (Ticket t in alleTickets)
            {
                // Volledige naam samenstellen zoals getoond in de UI.
                string volleNaam = t.MelderVoornaam + " " + t.MelderAchternaam;

                // Alleen toevoegen als de naam nog niet in de lijst staat.
                bool alAanwezig = false;
                foreach (string naam in gezieneNamen)
                {
                    if (naam == volleNaam)
                    {
                        alAanwezig = true;
                        break;
                    }
                }

                if (!alAanwezig)
                {
                    gezieneNamen.Add(volleNaam);
                    cmbMelderFilter.Items.Add(volleNaam);
                    cmbMelderNieuw.Items.Add(volleNaam);
                }
            }

            cmbMelderFilter.SelectedIndex = 0; // "Alle"
            if (cmbMelderNieuw.Items.Count > 0)
                cmbMelderNieuw.SelectedIndex = 0;

            // Handler weer aankoppelen.
            cmbMelderFilter.SelectionChanged += FilterGewijzigd;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // FILTEREN
        // ─────────────────────────────────────────────────────────────────────────────

        // Leest de huidige filterwaarden uit en vult lstTickets met de overeenkomende tickets.
        // Wordt aangeroepen bij elke wijziging van een filter of na het toevoegen/afsluiten.
        private void PasFiltersTo()
        {
            // Huidige filterwaarden ophalen.
            string gekozenPrioriteit = cmbPrioriteitFilter.SelectedItem as string;
            string gekozenMelder = cmbMelderFilter.SelectedItem as string;
            bool alleenOpen = chkAlleenOpen.IsChecked == true;

            lstTickets.Items.Clear();

            foreach (Ticket t in alleTickets)
            {
                // Prioriteit-filter: sla over als niet "Alle" en prioriteit komt niet overeen.
                if (gekozenPrioriteit != null
                    && gekozenPrioriteit != "Alle"
                    && t.Prioriteit.ToString() != gekozenPrioriteit)
                {
                    continue;
                }

                // Melder-filter: sla over als niet "Alle" en naam komt niet overeen.
                if (gekozenMelder != null
                    && gekozenMelder != "Alle")
                {
                    string volleNaam = t.MelderVoornaam + " " + t.MelderAchternaam;
                    if (volleNaam != gekozenMelder)
                        continue;
                }

                // Open-filter: sla afgesloten tickets over wanneer vinkje aan staat.
                if (alleenOpen && t.IsAfgesloten)
                    continue;

                // Ticket voldoet aan alle filters: toevoegen aan de ListBox.
                lstTickets.Items.Add(t);
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // EVENT HANDLERS – FILTERS
        // ─────────────────────────────────────────────────────────────────────────────

        // Wordt aangeroepen wanneer een filter-ComboBox of de CheckBox wijzigt.
        private void FilterGewijzigd(object sender, RoutedEventArgs e)
        {
            ToonFout(string.Empty);
            PasFiltersTo();
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // EVENT HANDLERS – LISTBOX SELECTIE
        // ─────────────────────────────────────────────────────────────────────────────

        // Toont de details van het geselecteerde ticket in txtDetails via GeefInfo().
        private void LstTickets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ToonFout(string.Empty);

            Ticket geselecteerd = lstTickets.SelectedItem as Ticket;

            if (geselecteerd != null)
                txtDetails.Text = geselecteerd.GeefInfo();
            else
                txtDetails.Text = string.Empty;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // EVENT HANDLERS – TICKET AFSLUITEN
        // ─────────────────────────────────────────────────────────────────────────────

        // Sluit het geselecteerde ticket af: zet IsAfgesloten op true en DatumAfgesloten
        // op het huidige tijdstip, slaat op naar CSV en herlaadt de weergave.
        private void BtnAfsluiten_Click(object sender, RoutedEventArgs e)
        {
            ToonFout(string.Empty);

            Ticket geselecteerd = lstTickets.SelectedItem as Ticket;

            // Validatie: er moet een ticket geselecteerd zijn.
            if (geselecteerd == null)
            {
                ToonFout("Selecteer eerst een ticket om af te sluiten.");
                return;
            }

            // Validatie: ticket mag nog niet afgesloten zijn.
            if (geselecteerd.IsAfgesloten)
            {
                ToonFout("Dit ticket is al afgesloten.");
                return;
            }

            // Ticket als afgesloten markeren met het huidige tijdstip.
            geselecteerd.IsAfgesloten = true;
            geselecteerd.DatumAfgesloten = DateTime.Now;

            // Wijziging opslaan naar het CSV-bestand.
            SlaOpNaarCsv();

            // Detailpaneel bijwerken met de nieuwe status.
            txtDetails.Text = geselecteerd.GeefInfo();

            // Filters opnieuw toepassen (het ticket kan uit de lijst verdwijnen
            // als "Alleen open tickets" aangevinkt staat).
            PasFiltersTo();
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // EVENT HANDLERS – NIEUW TICKET TOEVOEGEN
        // ─────────────────────────────────────────────────────────────────────────────

        // Leest het nieuwe-ticket-formulier uit, valideert de invoer,
        // maakt het juiste Ticket-object aan en slaat alles op.
        private void BtnToevoegen_Click(object sender, RoutedEventArgs e)
        {
            ToonFout(string.Empty);

            // ── Invoer ophalen ────────────────────────────────────────────────────
            string titel = txtTitel.Text.Trim();
            string melderNaam = cmbMelderNieuw.SelectedItem as string;
            string prioriteitTekst = cmbPrioriteitNieuw.SelectedItem as string;
            string extraInfo = txtExtraInfo.Text.Trim();

            // Type uitlezen via de geselecteerde ComboBoxItem.
            ComboBoxItem typeItem = cmbType.SelectedItem as ComboBoxItem;
            string typeNaam = typeItem != null ? typeItem.Content as string : string.Empty;

            // ── Validaties ────────────────────────────────────────────────────────
            if (string.IsNullOrWhiteSpace(titel))
            {
                ToonFout("Geef een titel in voor het nieuwe ticket.");
                return;
            }

            if (string.IsNullOrWhiteSpace(melderNaam))
            {
                ToonFout("Selecteer een melder.");
                return;
            }

            if (string.IsNullOrWhiteSpace(prioriteitTekst))
            {
                ToonFout("Selecteer een prioriteit.");
                return;
            }

            if (string.IsNullOrWhiteSpace(extraInfo))
            {
                // Het dynamische label ("Toestel" of "Applicatie") in de foutmelding gebruiken.
                ToonFout("Geef een waarde in voor het veld '" + lblExtraInfo.Text.TrimEnd(':') + "'.");
                return;
            }

            // Prioriteit omzetten van tekst naar enum-waarde.
            Prioriteit gekozenPrioriteit = Prioriteit.Normaal;
            foreach (Prioriteit p in Enum.GetValues(typeof(Prioriteit)))
            {
                if (p.ToString() == prioriteitTekst)
                {
                    gekozenPrioriteit = p;
                    break;
                }
            }

            // Melder opsplitsen in voornaam en achternaam op het eerste spatie-karakter.
            string melderVoornaam = string.Empty;
            string melderAchternaam = string.Empty;
            int spatiePositie = melderNaam.IndexOf(' ');
            if (spatiePositie >= 0)
            {
                melderVoornaam = melderNaam.Substring(0, spatiePositie);
                melderAchternaam = melderNaam.Substring(spatiePositie + 1);
            }
            else
            {
                // Geen spatie gevonden: volledige tekst als voornaam behandelen.
                melderVoornaam = melderNaam;
            }

            // MelderId afleiden uit bestaand ticket van deze persoon.
            string melderId = string.Empty;
            foreach (Ticket t in alleTickets)
            {
                if (t.MelderVoornaam == melderVoornaam && t.MelderAchternaam == melderAchternaam)
                {
                    melderId = t.MelderId;
                    break;
                }
            }

            // ── Juist ticket-type aanmaken ────────────────────────────────────────
            Ticket nieuwTicket;

            if (typeNaam == "Hardware")
            {
                HardwareTicket hw = new HardwareTicket();
                hw.Toestel = extraInfo;
                nieuwTicket = hw;
            }
            else
            {
                SoftwareTicket sw = new SoftwareTicket();
                sw.Applicatie = extraInfo;
                nieuwTicket = sw;
            }

            // Gemeenschappelijke velden invullen.
            nieuwTicket.Id = volgendId;
            volgendId++;
            nieuwTicket.Titel = titel;
            nieuwTicket.MelderVoornaam = melderVoornaam;
            nieuwTicket.MelderAchternaam = melderAchternaam;
            nieuwTicket.MelderId = melderId;
            nieuwTicket.Prioriteit = gekozenPrioriteit;
            nieuwTicket.IsAfgesloten = false;
            nieuwTicket.DatumAangemaakt = DateTime.Now;
            nieuwTicket.DatumAfgesloten = null;

            // Ticket aan de master-lijst toevoegen.
            alleTickets.Add(nieuwTicket);

            // Opslaan naar CSV en weergave vernieuwen.
            SlaOpNaarCsv();
            PasFiltersTo();

            // Formulier leegmaken na succesvolle toevoeging.
            txtTitel.Text = string.Empty;
            txtExtraInfo.Text = string.Empty;
            cmbPrioriteitNieuw.SelectedIndex = 1;
            cmbType.SelectedIndex = 0;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // EVENT HANDLERS – TYPE GEWIJZIGD
        // ─────────────────────────────────────────────────────────────────────────────

        // Past het dynamische label aan naast de extraInfo-TextBox:
        // "Toestel:" bij Hardware, "Applicatie:" bij Software.
        private void CmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // lblExtraInfo kan null zijn tijdens de initialisatie van de XAML-parser.
            if (lblExtraInfo == null)
                return;

            ComboBoxItem gekozenItem = cmbType.SelectedItem as ComboBoxItem;
            if (gekozenItem == null)
                return;

            if (gekozenItem.Content as string == "Hardware")
                lblExtraInfo.Text = "Toestel:";
            else
                lblExtraInfo.Text = "Applicatie:";
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // CSV OPSLAAN
        // ─────────────────────────────────────────────────────────────────────────────

        // Schrijft de volledige alleTickets-lijst terug naar het CSV-bestand.
        // Gebruikt exact hetzelfde formaat als het oorspronkelijke bestand.
        private void SlaOpNaarCsv()
        {
            // Datumnotatie die overeenkomt met TicketRepository.DatumFormaat.
            const string datumFormaat = "yyyy-MM-dd HHmm";

            List<string> regels = new List<string>();

            // Headerregel (zelfde als het originele bestand, inclusief aanhalingstekens).
            regels.Add("\"id;titel;melderVoornaam;melderAchternaam;melderId;prioriteit;isAfgesloten;type;extraInfo;datumAangemaakt;datumAfgesloten\"");

            foreach (Ticket t in alleTickets)
            {
                // Type en extraInfo bepalen op basis van de subklasse.
                string type = string.Empty;
                string extraInfo = string.Empty;

                if (t is HardwareTicket)
                {
                    type = "Hardware";
                    extraInfo = ((HardwareTicket)t).Toestel;
                }
                else if (t is SoftwareTicket)
                {
                    type = "Software";
                    extraInfo = ((SoftwareTicket)t).Applicatie;
                }

                // DatumAfgesloten leeg laten wanneer het ticket nog open is.
                string datumAfgesloten = string.Empty;
                if (t.IsAfgesloten && t.DatumAfgesloten.HasValue)
                    datumAfgesloten = t.DatumAfgesloten.Value.ToString(datumFormaat);

                // Rij samenstellen met puntkomma als scheidingsteken, omhuld door aanhalingstekens.
                string rij = string.Join(";",
                    t.Id.ToString(),
                    t.Titel,
                    t.MelderVoornaam,
                    t.MelderAchternaam,
                    t.MelderId,
                    t.Prioriteit.ToString(),
                    t.IsAfgesloten.ToString().ToLower(),
                    type,
                    extraInfo,
                    t.DatumAangemaakt.ToString(datumFormaat),
                    datumAfgesloten);

                regels.Add("\"" + rij + "\"");
            }

            File.WriteAllLines(volledigCsvPad, regels);
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // HULPFUNCTIES
        // ─────────────────────────────────────────────────────────────────────────────

        // Toont een foutmelding in de rode foutbalk onderaan het venster.
        // Geef een lege string door om de foutbalk te wissen.
        private void ToonFout(string bericht)
        {
            txtFout.Text = bericht;
        }
    }
}
