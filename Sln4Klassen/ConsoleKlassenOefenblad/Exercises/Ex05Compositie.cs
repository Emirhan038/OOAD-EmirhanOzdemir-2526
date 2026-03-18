using ConsoleKlassenOefenblad.Exercises.Classes;

namespace ConsoleKlassenOefenblad.Exercises;

internal class Ex05Compositie
{
    public static void Run()
    {
        Console.WriteLine("\nOefening 5: compositie");
        Console.WriteLine("-------------");

        List<Product> producten = new List<Product>
        {
            new Product { ProductId = 9112, Naam = "Laptop", Beschrijving = "14-inch, 16GB RAM", Prijs = 999.99m, Voorraad = 12 },
            new Product { ProductId = 2876, Naam = "Bureaulamp", Beschrijving = "LED, dimbaar", Prijs = 34.50m, Voorraad = 0 },
            new Product { ProductId = 3033, Naam = "Rugzak", Beschrijving = "Waterbestendig, 30L", Prijs = 59.95m, Voorraad = 8 },
            new Product { ProductId = 4441, Naam = "Koptelefoon", Beschrijving = "Noise-cancelling", Prijs = 149.00m, Voorraad = 3 },
            new Product { ProductId = 5508, Naam = "Muis", Beschrijving = "Draadloos, ergonomisch", Prijs = 29.99m, Voorraad = 20 },
            new Product { ProductId = 6274, Naam = "Toetsenbord", Beschrijving = "Mechanisch, RGB", Prijs = 89.95m, Voorraad = 7 },
            new Product { ProductId = 7390, Naam = "Webcam", Beschrijving = "Full HD, 1080p", Prijs = 64.50m, Voorraad = 5 },
            new Product { ProductId = 8115, Naam = "USB-hub", Beschrijving = "7 poorten, USB-C", Prijs = 24.99m, Voorraad = 0 },
            new Product { ProductId = 8823, Naam = "Monitor", Beschrijving = "27-inch, 4K IPS", Prijs = 449.00m, Voorraad = 4 },
            new Product { ProductId = 9647, Naam = "Telefoonhouder", Beschrijving = "Verstelbaar, bureaumodel", Prijs = 14.75m, Voorraad = 15 },
        };

        Bestelling bestelling1 = new Bestelling
        {
            BestellingId = 1,
            KlantNaam = "Amara Diallo",
            Producten = new List<Product> { producten[0], producten[2], producten[6] }
        };

        Bestelling bestelling2 = new Bestelling
        {
            BestellingId = 2,
            KlantNaam = "Yuna Kim",
            Producten = new List<Product> { producten[0], producten[8] }
        };

        Console.WriteLine($"Totaalbedrag bestelling 1: {bestelling1.TotaalBedrag:F2}");

        Console.WriteLine($"details bestelling 1: {bestelling1}");
        foreach (Product p in bestelling1.Producten)
        {
            Console.WriteLine($"  \u2013 {p}");
        }

        foreach (Product p in bestelling2.Producten)
        {
            p.Korting = 5;
        }

        Console.WriteLine($"details bestelling 2: {bestelling2}");
        foreach (Product p in bestelling2.Producten)
        {
            Console.WriteLine($"  \u2013 {p}");
        }
    }
}