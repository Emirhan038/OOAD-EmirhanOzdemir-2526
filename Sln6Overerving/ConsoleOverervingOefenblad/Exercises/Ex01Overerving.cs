using ConsoleOverervingOefenblad.Exercises.Classes.Klant;

namespace ConsoleOverervingOefenblad.Exercises
{
    /// <summary>
    /// Oefening 1 — Basisoefening op overerving: ProfessioneleKlant erft over van Klant
    /// </summary>
    internal static class Ex01Overerving
    {
        public static void Run()
        {
            Console.WriteLine("Oefening 1: Overerving");
            Console.WriteLine("-------------");

            List<Klant> klanten = new List<Klant>();

            klanten.Add(new Klant { Naam = "Lotte Peeters", Email = "lotte.peeters@mail.be" });
            klanten.Add(new Klant { Naam = "Youssef El Amrani", Email = "youssef.elamrani@outlook.com" });
            klanten.Add(new Klant { Naam = "Chloé Van den Broeck", Email = "chloe.vdbroeck@gmail.com" });
            klanten.Add(new Klant { Naam = "Milan De Vos", Email = "milan.devos@yahoo.com" });

            klanten.Add(new ProfessioneleKlant
            {
                Naam = "Tech Solutions BV",
                Email = "info@techsolutions.be",
                BedrijfsNaam = "Tech Solutions",
                BTWNummer = "BE0123456789"
            });

            klanten.Add(new ProfessioneleKlant
            {
                Naam = "BuildCorp NV",
                Email = "contact@buildcorp.be",
                BedrijfsNaam = "BuildCorp",
                BTWNummer = "BE9876543210"
            });

            Console.WriteLine("Overzicht klanten:");
            foreach (Klant klant in klanten)
            {
                Console.WriteLine($"- {klant}");
            }
        }
    }
}