using ConsoleOverervingOefenblad.Exercises;

namespace ConsoleOverervingOefenblad;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        bool stoppen = false;
        while (!stoppen)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"
=== Oefenblad: Overerving ===

1. Open 'Exercises/Assignments/index.html' in de browser; daar vind je de opdrachten.
2. Elke oefening vind je in de map 'Exercises'.
3. De bijhorende klassen werk je uit in de map 'Exercises/Classes'
4. Start het programma en kies het nummer van de oefening om de testcode uit te voeren.
5. Controleer of de output exact klopt met de screenshot!");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("Kies een oefening om uit te voeren:");
            Console.WriteLine();
            Console.WriteLine("1 - Eenvoudige overerving: ProfessioneleKlant");
            Console.WriteLine("2 - Abstracte klasse: CatalogusItem");
            Console.WriteLine("3 - Abstracte members: ValidatieRegel");
            Console.WriteLine("4 - is en as: Workout");
            Console.WriteLine("5 - base: Manager");
            Console.WriteLine();
            Console.Write("Je keuze (enter om te stoppen): ");
            char choice = Console.ReadKey().KeyChar;
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            switch (choice)
            {
                case '1': Ex01Overerving.Run(); break;
                case '2': Ex02AbstracteKlassen.Run(); break;
                case '3': Ex03AbstracteMembers.Run(); break;
                case '4': Ex04IsAs.Run(); break;
                case '5': Ex05Base.Run(); break;
                default: stoppen = true; break;
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("druk een toets om verder te gaan...");
            Console.ReadKey();
        }
    }
}
