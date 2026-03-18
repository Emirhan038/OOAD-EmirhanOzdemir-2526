using System;

class Program
{
    static void Main(string[] args)
    {
        const int AANTAL_KAARTEN_HAND = 5;

        Deck deck = new Deck();
        deck.Schudden();

        Speler spelerHans = new Speler("Hans");
        Speler spelerRogier = new Speler("Rogier");

        for (int i = 0; i < AANTAL_KAARTEN_HAND; i++)
        {
            spelerHans.Kaarten.Add(deck.NeemKaart());
            spelerRogier.Kaarten.Add(deck.NeemKaart());
        }

        double puntenHans = 0;
        double puntenRogier = 0;

        while (spelerHans.HeeftNogKaarten && spelerRogier.HeeftNogKaarten)
        {
            Kaart kaart1 = spelerHans.LegKaart();
            Kaart kaart2 = spelerRogier.LegKaart();

            Console.WriteLine($"Hans legt {kaart1.Kleur}{kaart1.Nummer}");
            Console.WriteLine($"Rogier legt {kaart2.Kleur}{kaart2.Nummer}");

            if (kaart1.Nummer > kaart2.Nummer)
                puntenHans++;
            else if (kaart1.Nummer < kaart2.Nummer)
                puntenRogier++;
            else
            {
                puntenHans += 0.5;
                puntenRogier += 0.5;
            }

            Console.WriteLine($"stand: Hans {puntenHans} - Rogier {puntenRogier}");
        }

        if (puntenRogier == puntenHans)
            Console.WriteLine("\ngelijkspel!");
        else if (puntenRogier > puntenHans)
            Console.WriteLine("\nRogier wint!");
        else
            Console.WriteLine("\nHans wint!");

        Console.ReadKey();
    }
}