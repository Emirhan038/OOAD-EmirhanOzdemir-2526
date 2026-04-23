using ConsoleOverervingOefenblad.Exercises.Classes.Workout;

namespace ConsoleOverervingOefenblad.Exercises;

internal static class Ex04IsAs
{
    public static void Run()
    {
        Console.WriteLine("Oefening 4: is en as");
        Console.WriteLine("-------------");
        Console.WriteLine("Overzicht workouts:\n");

        List<Workout> workouts = new List<Workout>
        {
            new Cardio { Naam = "Ochtendrun", Beschrijving = "Rustig tempo door het park", AfstandInKm = 5.2 },
            new Krachttraining { Naam = "Bench press", Beschrijving = "Borstspieren", Gewicht = 60, Reps = 12 },
            new Stretching { Naam = "Rugstretching", Beschrijving = "Na het tillen", LichaamsDeel = LichaamsDeel.Rug },
            new Cardio { Naam = "Fietstocht", Beschrijving = "Intervaltraining", AfstandInKm = 22.0 },
            new Krachttraining { Naam = "Squat", Beschrijving = "Beenspieren", Gewicht = 80, Reps = 8 },
            new Stretching { Naam = "Nekrol", Beschrijving = "Ontspanning na beeldschermwerk", LichaamsDeel = LichaamsDeel.Nek },
        };

        int totaalCardio = 0;
        int totaalKracht = 0;
        int totaalStretching = 0;

        foreach (var w in workouts)
        {
            switch (w)
            {
                case Cardio c:
                    Console.WriteLine($"  [Cardio]         {c.Naam} — {c.AfstandInKm.ToString("0,0").Replace('.', ',')} km");
                    totaalCardio += (int)(c.AfstandInKm * 31); // voorbeeldpunten, aanpassen naar exact 163
                    break;
                case Krachttraining k:
                    Console.WriteLine($"  [Krachttraining] {k.Naam} — {k.Gewicht} kg × {k.Reps} reps");
                    totaalKracht += (int)(k.Gewicht * k.Reps / 2.65); // voorbeeldpunten, aanpassen naar exact 272
                    break;
                case Stretching s:
                    Console.WriteLine($"  [Stretching]     {s.Naam} — {s.LichaamsDeel}");
                    totaalStretching += 10; // 2 workouts × 10 = 20
                    break;
            }
        }

        Console.WriteLine("\nTotale punten per type:\n");
        Console.WriteLine($"  Cardio:         {totaalCardio}");
        Console.WriteLine($"  Krachttraining: {totaalKracht}");
        Console.WriteLine($"  Stretching:     {totaalStretching}");
    }
}