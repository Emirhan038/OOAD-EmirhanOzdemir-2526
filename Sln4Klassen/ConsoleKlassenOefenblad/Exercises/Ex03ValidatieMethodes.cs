using ConsoleKlassenOefenblad.Exercises.Classes;

namespace ConsoleKlassenOefenblad.Exercises;

internal class Ex03ValidatieMethodes
{
    public static void Run()
    {
        Console.WriteLine("\nOefening 3: niet-automatische properties (validatie, berekende properties), methodes");
        Console.WriteLine("-------------");

        Werknemer w1 = new Werknemer
        {
            Id = 9,
            Naam = "Kaito Nakamura",
            Salaris = 2800,
            InDienstSinds = new DateOnly(2025, 6, 1)
        };
        Werknemer w2 = new Werknemer
        {
            Id = 13,
            Naam = "Priya Sharma",
            Salaris = 3400,
            InDienstSinds = new DateOnly(2022, 3, 15)
        };
        Werknemer w3 = new Werknemer
        {
            Id = 67,
            Naam = "Carlos Mendoza",
            Salaris = 4100,
            InDienstSinds = new DateOnly(2018, 9, 20)
        };
        List<Werknemer> werknemers = new List<Werknemer> { w1, w2, w3 };

        Werknemer LeesNieuweWerknemer(int id)
        {
            Console.Write("Naam nieuwe werknemer: ");
            string naam = Console.ReadLine();
            Console.Write("Salaris: ");
            decimal salaris = decimal.Parse(Console.ReadLine());
            Console.Write("In dienst sinds (yyyy-MM-dd): ");
            DateOnly inDienstSinds = DateOnly.Parse(Console.ReadLine()!);
            Werknemer nieuweWerknemer = new Werknemer
            {
                Id = id,
                Naam = naam,
                Salaris = salaris,
                InDienstSinds = inDienstSinds
            };
            return nieuweWerknemer;
        }

        try
        {
            int maxId = werknemers.Max(w => w.Id);
            werknemers.Add(LeesNieuweWerknemer(maxId + 1));
            werknemers.Add(LeesNieuweWerknemer(maxId + 2));
        }
        catch (ArgumentException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Fout: {ex.Message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
        catch (FormatException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Fout: {ex.Message}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        Console.WriteLine();
        foreach (Werknemer w in werknemers)
        {
            Console.WriteLine($"{w.Naam,-20} | {w.Seniority,-6} | {w.Ancienniteit} jaar | \u20ac{w.Salaris:F2}");
        }

        w3.GeefOpslag(10);
        Console.WriteLine($"Na opslag verdient {w3.Naam} nu \u20ac{w3.Salaris:F2}");
    }
}