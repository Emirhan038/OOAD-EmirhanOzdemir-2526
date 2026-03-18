using System;
using System.Collections.Generic;

public class Deck
{
    private List<Kaart> kaarten;
    private Random random = new Random();

    public Deck()
    {
        kaarten = new List<Kaart>();

        string[] kleuren = { "♠", "♥", "♦", "♣" };

        for (int i = 0; i < kleuren.Length; i++)
        {
            for (int nummer = 1; nummer <= 13; nummer++)
            {
                kaarten.Add(new Kaart(kleuren[i], nummer));
            }
        }
    }

    public void Schudden()
    {
        for (int i = 0; i < kaarten.Count; i++)
        {
            int j = random.Next(kaarten.Count);
            Kaart temp = kaarten[i];
            kaarten[i] = kaarten[j];
            kaarten[j] = temp;
        }
    }

    public Kaart NeemKaart()
    {
        if (kaarten.Count == 0)
            return null;

        Kaart kaart = kaarten[0];
        kaarten.RemoveAt(0);
        return kaart;
    }
}