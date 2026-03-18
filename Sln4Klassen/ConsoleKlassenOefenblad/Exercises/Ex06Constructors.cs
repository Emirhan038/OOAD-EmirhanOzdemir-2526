using ConsoleKlassenOefenblad.Exercises.Classes;

namespace ConsoleKlassenOefenblad.Exercises;

internal class Ex06Constructors
{
    public static void Run()
    {
        Console.WriteLine("\nOefening 6: constructors");
        Console.WriteLine("-------------");

        ProfielInfo p1 = new ProfielInfo(1, "kaito99", "kaito@example.com");
        ProfielInfo p2 = new ProfielInfo(2, "priya_s", "priya@example.com", "Priya", "Sharma", "Softwareontwikkelaar uit Gent.", "https://priya.dev", true);
        ProfielInfo p3 = new ProfielInfo(3, "carlos_m", "carlos@example.com", "Carlos", "Mendoza", "", "", false);

        List<ProfielInfo> profielen = new List<ProfielInfo> { p1, p2, p3 };
        foreach (ProfielInfo p in profielen)
        {
            Console.WriteLine($"{p} | profiel is {(p.IsVolledig ? "volledig" : "onvolledig")}");
        }
    }
}