using ConsoleKlassenOefenblad.Exercises.Classes;

namespace ConsoleKlassenOefenblad.Exercises;

internal class Ex04ToString
{
    public static void Run()
    {
        Console.WriteLine("\nOefening 4: toString()");
        Console.WriteLine("-------------");

        List<Product> producten = new List<Product>
        {
            new Product { ProductId = 9112, Naam = "Laptop", Beschrijving = "14-inch, 16GB RAM", Prijs = 999.99m, Voorraad = 12 },
            new Product { ProductId = 2876, Naam = "Bureaulamp", Beschrijving = "LED, dimbaar", Prijs = 34.50m, Voorraad = 0 },
            new Product { ProductId = 3033, Naam = "Rugzak", Beschrijving = "Waterbestendig, 30L", Prijs = 59.95m, Voorraad = 8 },
            new Product { ProductId = 4441, Naam = "Koptelefoon", Beschrijving = "Noise-cancelling", Prijs = 149.00m, Voorraad = 3 },
        };

        for (int i = 0; i < producten.Count; i++)
        {
            Product p = producten[i];
            Console.WriteLine($"product #{i}: {p}");
        }
    }
}