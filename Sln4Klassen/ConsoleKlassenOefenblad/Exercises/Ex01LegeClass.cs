using ConsoleKlassenOefenblad.Exercises.Classes;

namespace ConsoleKlassenOefenblad.Exercises;

internal class Ex01LegeClass
{
    public static void Run()
    {
        Console.WriteLine("Oefening 1: lege klasse");
        Console.WriteLine("-------------");

        List<Knikker> potje = new List<Knikker>();

        for (int i = 0; i < 10; i++)
        {
            potje.Add(new Knikker());
        }

        Console.WriteLine($"Er zitten {potje.Count} knikkers in het potje.");
    }
}