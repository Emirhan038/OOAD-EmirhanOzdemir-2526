using ConsoleKlassenOefenblad.Exercises.Classes;

namespace ConsoleKlassenOefenblad.Exercises;

internal class Ex02Properties
{
    public static void Run()
    {
        Console.WriteLine("\nOefening 2: properties, standaardwaarden, object initializer syntax");
        Console.WriteLine("-------------");

        Recept pastaCarbonara = new Recept();
        pastaCarbonara.Titel = "Pasta Carbonara";
        pastaCarbonara.Rating = 4;
        pastaCarbonara.Ingredienten = new List<string> { "Pasta", "Eieren", "Spek", "Parmezaanse kaas" };

        Recept lasagne = new Recept
        {
            Titel = "Lasagne",
            Rating = 5,
            IsVegetarisch = true,
            Ingredienten = new List<string> { "Lasagnebladen", "Tomatensaus", "Courgette", "Aubergine", "Mozzarella" }
        };

        Recept saladeNicoise = new Recept
        {
            Titel = "Salade Ni\u00e7oise",
            Rating = 4,
            IsVegetarisch = true,
            Ingredienten = new List<string> { "Sla", "Tonijn", "Eieren", "Pindakaas", "Olijven", "Tomaten" }
        };

        saladeNicoise.Ingredienten.Remove("Pindakaas");
        saladeNicoise.IsVegetarisch = false;

        List<Recept> kookboek = new List<Recept> { pastaCarbonara, lasagne, saladeNicoise };

        int aantalVegetarisch = kookboek.Count(r => r.IsVegetarisch);
        double gemiddeldeRating = kookboek.Average(r => r.Rating);

        Console.WriteLine($"Aantal vegetarische recepten in het kookboek: {aantalVegetarisch}");
        Console.WriteLine($"De gemiddelde rating is {gemiddeldeRating:F1}.");
    }
}