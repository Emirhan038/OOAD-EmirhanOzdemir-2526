ik heb gewerkt in de project file van de project van ooad zodat hij alles heeft gedaan zoals hij daar deed geen verboden dingen gebruikt ik heb deze protm geggeevn en hij wist wat hij al de rest moet doen 

JE WERKT IN DE SOLUTTION VAN slnExamen is al gemaakt 

DE ZWARTE FOTOS EN DEZE TEKST ONDER IS WAT ER ONLINE WNR IK DE EXAEMN START ER IS 
P04.Ai.Bestand - helpdesk 40min
10.0p1
Dit is een opgave die je volledig met AI mag uitwerken. Je mag dus vrij gebruik maken van alle mogelijke AI tools (Claude code, chatGPT, …). Deze oefening is volledig open boek, alle AI tools zijn toegelaten, maar uiteraard zijn alle sharing- en communicatieprogramma’s en tools verboden; zie dat ze volledig afgesloten zijn voor je aan de oefening begint.
Opgave
Je werkt in je repository in de bestaande solution SlnExamen (EP2) of SlnTweedeZit (EP3). Open SlnExamen.slnx, en voeg daarin je projecten toe. Screenshot voor eerste zit:
 Het moet er exact zo uitzien! De opdrachtomschrijving en richtlijnen krijg je op papier.
Indienen
Voeg het volgende toe aan de root van je project:

1. een agent instruction file
2. een markdown document documentatie.md met je initiële prompt, door AI gegenereerde plan van aanpak, overzicht gebruikte agents + bondige samenvatting van elk gespreksverloop (je mag AI gebruiken om te helpen bij de formulering, maar de tekst moet wel van jou zijn)
Commit en push je oplossing naar je repository. Maak daarna één ZIP-bestand van je oplossing. Selecteer alle bestanden van de oefening, en klik op de rechtermuisknop:
Dit .zip bestand upload je hier op ANS.
Niet geupload op ANS: NA
Niet gepushed op Github: -4/20
Foutief gepushed op Github (verkeerde folder): -2/20




context van op pg
OOAD praktijkopgave - Help desk
je maakt een eenvoudige toepassing voor een interne IT-helpdesk met ai medewerkets melden problemen met hardware of software een helpdeskmedewerker kan tickets raadplegen filters nieuwe tickets registeren en ticketen afsluiten let op je  let op je blijft verantwoorde vvoor de keuzes en code kwalieit

EVALUTATIE
de mock up moeten gevolgt zijn je mag wel layou en desing naar eigen smaak aanpassen
het gerbuik van classes is over de hele lijn verplicht er is gebruik gemaakt van een class library en je app communisern effectief met de csv besteand via de class library
gebruik enkel geziene technieke uit de cursus op onderelen waar je niet toegelaten technieken gerbruik(databinding, datagrid,gridvview ,listvieuw linq tuples case guard , asny/await dynamic var,expando objecten , invoke strucks, type switches, user controls, out parameter..)
krijg je een nul DAAROM HEEL ZEKER ZIJN DAT JE NIKS VERBODEN GERBUIKT DIE NIET GEZIEN WAS IN DE CURSUS



OPGAVE 
de toepassing bestaat ui een wpf project class library we werken niet met een databank maar met een csv besten waarbij elke rij een voorsteld record met gegeven gesceiden door een teken als een komma of een puntkomma voor een bestaande tickets is een csv besteand aangelever in ans met een puntkomma of scheidingteken kopier dit naar je solutionn: de namen van de kollomen vind je in de eerste rij


Datum formaat yyyy-mm-dd HHmm

Project setup 
je werk in je reposetery in slmexamen verplicht? maak daarin twee projeten 
1 een wpf app (.net10) WpfHelpdesk
2een class library  (.net10) CLHelpdesk

class library
maak nu de class library voorzie volgende enum en klassen met alwaste de meest courante propreties construcors en nodige methode

enum ticketPriotiteit
waarden Laag, Normaal, Hoog

klasse Medewerk
proreties 
Id, Voornaam, Achternaam, lijst van Tickets

basisklasse Tickets
propreties Id,Titel,Melder,Prioriteit, isAfgesloten,DatumaAanmaak, DatumAfgeslote
methode;minstens geefinfo(), tegebruiken in de detail-weergave; en ToString() wordt gerbruikt in de listbox en geeft een korte duidelijke weergave

klassen HardwareTicket en SoftwareTickets(afgeleid van Tickets)
leid zelf de bijkomemde propeties af uit onderstaande screenshots
overschrijft de Geefinfo() methode uit de basisklass naar voorbeeld van onderstaande screen shots ik heb zelf fotos genomen van de screenshot je zal de hebben met daarin als de tekst 


ik heb al de cvs bestand in de map gezet zie ook ss 

geef mij in 2-3 promts de hele appliatie

daarna 1 promt voor controle 

zorg dat je alles echt perffect doet zo claude code doet het goed 

je moet geen md file geen plannig gwn bouwin promt dankjewel een volg alles aub



PROMT 1 van ai 
Werk in de bestaande solution SlnExamen.slnx. Voeg twee .NET 10-projecten toe:
- Class Library `CLHelpdesk`
- WPF App `WpfHelpdesk` met projectreferentie naar CLHelpdesk

Het bestand helpdesk_tickets.csv staat al in de solution folder. Voeg het in WpfHelpdesk.csproj toe als Content met CopyToOutputDirectory=Always.

ABSOLUUT NIET gebruiken (= 0 punten op dat onderdeel): var, LINQ, data binding, DataGrid/GridView/ListView, async/await, out parameters incl. int.TryParse en DateTime.TryParse, tuples, case guards, dynamic, expando, Invoke, structs, type switches, UserControls, MessageBox. Altijd expliciete types, for/foreach/while-lussen, parse binnen try-catch (FormatException).

CSV-specificaties (LEES EERST helpdesk_tickets.csv om te bevestigen, hier de structuur):
- Separator `;`
- ELKE regel staat ingepakt in dubbele aanhalingstekens: `"id;titel;...;datumAfgesloten"`. Bij lezen: regel.Trim('"') vóór split. Bij schrijven: regel opnieuw inpakken met `"..."`.
- Eerste regel = header: id;titel;melderVoornaam;melderAchternaam;melderId;prioriteit;isAfgesloten;type;extraInfo;datumAangemaakt;datumAfgesloten
- Datumformaat: `yyyy-MM-dd HHmm` (GEEN dubbele punt). Parse met DateTime.ParseExact(s, "yyyy-MM-dd HHmm", CultureInfo.InvariantCulture); format bij schrijven met dezelfde string.
- datumAfgesloten is leeg als ticket open is.
- type = "Hardware" → HardwareTicket; "Software" → SoftwareTicket.
- extraInfo mapped naar Toestel (Hardware) of Applicatie (Software).
- isAfgesloten = "true"/"false" (lowercase).

Maak in CLHelpdesk:

1) enum `TicketPrioriteit { Laag, Normaal, Hoog }`

2) klasse `Medewerker` met properties: string Id, string Voornaam, string Achternaam, List<Ticket> Tickets. Twee constructors (leeg + volledig). override ToString() → "Voornaam Achternaam (Id)".
   Statische methode `public static List<Medewerker> GeefAlleMedewerkers()`: lees de CSV, haal unieke melders op (één per melderId, koppel hun tickets aan de lijst), return list.

3) basisklasse `Ticket` (NIET abstract — gebruik virtuele methode) met properties: int Id, string Titel, Medewerker Melder, TicketPrioriteit Prioriteit, bool IsAfgesloten, DateTime DatumAangemaakt, DateTime? DatumAfgesloten. Twee constructors (leeg + volledig).
   - public virtual string GeefInfo() — gemeenschappelijke regels (Titel, Melder met formaat "Voornaam Achternaam (Id)", Prioriteit, Status "Open"/"Afgesloten", Aangemaakt in `dd/MM/yyyy HH:mm`, en als afgesloten ook Afgesloten-regel).
   - override ToString() → kort, bv. `[Prioriteit] Titel — Voornaam Achternaam`.
   - Statisch: `public static List<Ticket> GeefAlleTickets()` (leest CSV en bouwt HardwareTicket of SoftwareTicket per regel; koppelt Melder-instantie).
   - Statisch: `public static void OpslaanAlleTickets(List<Ticket> tickets)` (herschrijft hele CSV incl. header, met de outer quotes).
   - Instance: `public void Toevoegen()` (zet Id = max+1, append aan lijst uit GeefAlleTickets en bewaar).
   - Instance: `public void Afsluiten()` (IsAfgesloten=true, DatumAfgesloten=DateTime.Now, herschrijf CSV).

4) `HardwareTicket : Ticket` — extra property `string Toestel`. override GeefInfo(): voeg "Type: Hardware" en "Toestel: ..." toe (en eventueel afgesloten-regel).

5) `SoftwareTicket : Ticket` — extra property `string Applicatie`. override GeefInfo(): "Type: Software" en "Applicatie: ...".

Alle code in CLHelpdesk — geen CSV-IO in WpfHelpdesk. Rijkelijk Nederlands commentaar. Compileer. Doe niets buiten dit scope. Geen MD-files.


PROMT2
Bouw nu WpfHelpdesk MainWindow volledig volgens de mockup. Eén MainWindow, geen Frame/Page nodig.

Layout (Grid met rijen/kolommen):
- BOVEN-LINKS: Filters (Prioriteit ComboBox met items "Alle","Laag","Normaal","Hoog"; Melder ComboBox met "Alle" + alle medewerkers; CheckBox "Alleen open tickets"). Eronder: ListBox `lstTickets`.
- BOVEN-RECHTS: TextBlock `txtDetails` (wrapping, monospace optioneel) voor GeefInfo(). Eronder: Button "Ticket afsluiten" (`btnAfsluiten`).
- ONDERAAN: groepje "Nieuw ticket" met TextBox Titel, ComboBox Melder, ComboBox Prioriteit, ComboBox Type (Hardware/Software), TextBox extraInfo met Label dat dynamisch "Toestel" of "Applicatie" toont, Button "Toevoegen" (`btnToevoegen`).
- Onderaan een TextBlock `txtFout` voor foutmeldingen (rood).

Code-behind (geen binding, geen DataGrid, geen var, geen LINQ, geen out/TryParse, geen MessageBox):

In constructor (na InitializeComponent):
- Vul `cmbPrioriteitFilter` met "Alle" + enum-waarden (foreach over Enum.GetValues(typeof(TicketPrioriteit))).
- Vul `cmbMelderFilter` met "Alle" + Medewerker.GeefAlleMedewerkers().
- Vul `cmbPrioriteitNieuw` met enum-waarden.
- Vul `cmbMelderNieuw` met medewerkers (zonder "Alle").
- Vul `cmbTypeNieuw` met "Hardware" en "Software".
- Roep `HerlaadOverzicht()` aan.
- Roep `WerkKnopAfsluitenBij()` aan.

`private void HerlaadOverzicht()`:
- Lees alle tickets via Ticket.GeefAlleTickets() (try-catch FormatException/IOException → toon in txtFout).
- Filter: loop met foreach door de lijst, sla items op in een nieuwe List<Ticket> volgens de drie filters (Prioriteit-filter, Melder-filter op melderId, en als checkbox aan → enkel IsAfgesloten==false).
- Maak lstTickets.Items leeg en voeg de gefilterde tickets één voor één toe (ListBox toont ToString()).

Event handlers:
- SelectionChanged van de drie filters + Checked/Unchecked van de checkbox → HerlaadOverzicht().
- lstTickets.SelectionChanged → als geselecteerd: txtDetails.Text = ticket.GeefInfo(); anders leeg. WerkKnopAfsluitenBij().
- cmbTypeNieuw.SelectionChanged → pas Label naast extraInfo-textbox aan ("Toestel" als Hardware, "Applicatie" als Software).
- btnAfsluiten.Click → cast geselecteerd item naar Ticket, ticket.Afsluiten() in try-catch. HerlaadOverzicht().
- btnToevoegen.Click → valideer:
   * Titel.Trim().Length >= 5 anders "Titel moet minstens 5 tekens bevatten."
   * Melder geselecteerd
   * Prioriteit geselecteerd
   * Type geselecteerd
   * extraInfo niet leeg
   Bij fout: txtFout.Text = melding; return.
   Bij ok: maak HardwareTicket of SoftwareTicket aan (op basis van cmbTypeNieuw), vul properties, ticket.Toevoegen() in try-catch, clear het form, HerlaadOverzicht(), txtFout.Text = "".

`private void WerkKnopAfsluitenBij()`:
- btnAfsluiten.IsEnabled = (lstTickets.SelectedItem != null && ((Ticket)lstTickets.SelectedItem).IsAfgesloten == false).

REGELS opnieuw: nooit var, LINQ, binding, DataGrid/ListView, async/await, out, TryParse, MessageBox, tuples, dynamic, struct, type switch, UserControl. Foutmeldingen in txtFout. try-catch rond elke CSV/file-operatie. Rijkelijk Nederlands commentaar.

Bouw en compileer. Doe niets buiten dit scope. Geen MD-files.



PROMT 3 CONTROLE
Ask mode. Wijzig niets. Controleer de hele SlnExamen-solution en geef een rapport met bestand + regelnummer + probleem voor elke vondst:

1) Verboden technieken aanwezig? Scan op: var, LINQ-methodes (Where/Select/Any/ToList/etc.), `using System.Linq`, data binding (Binding=..., DataContext), DataGrid/GridView/ListView, async/await/Task, out, TryParse, tuples (ValueTuple, (a,b)), case guards (when), dynamic, expando, Invoke, structs (struct keyword), type switches (switch op type), UserControl, MessageBox.
2) Staat alle CSV-code uitsluitend in CLHelpdesk? (geen File.* of CSV-parsing in WpfHelpdesk code-behind)
3) Zit de CRUD volledig in de klassen (Ticket/Medewerker) zelf? Geen aparte datalayer/datacontext?
4) Klopt de overerving: Ticket-basis met virtual GeefInfo(), HardwareTicket en SoftwareTicket overschrijven met override en hebben extra property Toestel resp. Applicatie?
5) Heeft Medewerker een List<Ticket> Tickets?
6) Datumformaat exact `yyyy-MM-dd HHmm` (geen `:` tussen uur/minuten) zowel bij lezen (ParseExact) als schrijven?
7) Wordt elke CSV-regel correct met outer quotes gelezen (.Trim('"')) én geschreven (regel opnieuw inpakken in `"..."`)?
8) Functioneel:
   - App start zonder crash, lstTickets is gevuld.
   - Filters Prioriteit/Melder/"Alleen open" werken correct.
   - Selectie toont GeefInfo() in txtDetails.
   - btnAfsluiten is uitgeschakeld bij geen selectie of als ticket al afgesloten is.
   - Nieuw ticket: validatie werkt, bij fout zichtbare melding in txtFout, bij ok wordt correct HardwareTicket of SoftwareTicket aangemaakt, CSV bevat de nieuwe regel met outer quotes, overzicht herlaadt.
9) Bouwt zonder warnings/errors.

Geef enkel het rapport in lijstvorm. Repareer niets — ik beslis of we fixen.


HET IS VRIJ GOED GEDAAN EN SMOOTH GEEN PROBLEMEN 