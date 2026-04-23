using ConsoleOverervingOefenblad.Exercises.Classes.CatalogusItem;

namespace ConsoleOverervingOefenblad.Exercises;

internal static class Ex02AbstracteKlassen
{
    public static void Run()
    {
        Console.WriteLine("Oefening 2: eigen basisklasse en afgeleiden");
        Console.WriteLine("-------------");

        Boek roman = new Boek
        {
            Titel = "De hemel is altijd paars",
            InventarisNummer = "B-10042",
            Auteur = "Ruta Sepetys",
            AantalPaginas = 412,
        };

        FilmDvd film = new FilmDvd
        {
            Titel = "Koko",
            InventarisNummer = "D-08821",
            Regisseur = "Taika Waititi",
            SpeelduurMinuten = 101,
        };

        EBoek handleiding = new EBoek
        {
            Titel = "C# in een notendop",
            InventarisNummer = "E-00003",
            BestandsgrootteMB = 4.2,
            Formaat = EboekFormaat.Pdf,
        };

        Console.WriteLine($"[{roman.InventarisNummer}] {roman.Titel}");
        Console.WriteLine($"  Boek — {roman.Auteur}, {roman.AantalPaginas} pagina's");
        Console.WriteLine();

        Console.WriteLine($"[{film.InventarisNummer}] {film.Titel}");
        Console.WriteLine($"  DVD — {film.Regisseur}, {film.SpeelduurMinuten} min");
        Console.WriteLine();

        Console.WriteLine($"[{handleiding.InventarisNummer}] {handleiding.Titel}");
        Console.WriteLine($"  E-book — {handleiding.Formaat}, {handleiding.BestandsgrootteMB:F1} MB");
    }
}