using ConsoleOverervingOefenblad.Exercises.Classes.Medewerker;

namespace ConsoleOverervingOefenblad.Exercises;

internal static class Ex05Base
{
    public static void Run()
    {
        Console.WriteLine("Oefening 5: base");
        Console.WriteLine("-------------");

        List<Medewerker> personeel = new List<Medewerker>
        {
            new Medewerker("Lotte Peeters", "Marketing"),
            new Manager("Niels Verhoeven", "IT", 8),
            new Medewerker("Youssef El Amrani", "Sales"),
            new Manager("Sofia Dimitriou", "HR", 5),
            new Medewerker("Chloé Van den Broeck", "Finance"),
        };

        foreach (Medewerker m in personeel)
        {
            Console.WriteLine(m);
        }
    }
}