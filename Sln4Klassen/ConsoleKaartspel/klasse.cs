using System.Collections.Generic;

public class Speler
{
    public string Naam { get; set; }
    public List<Kaart> Kaarten { get; set; }

    public Speler(string naam)
    {
        Naam = naam;
        Kaarten = new List<Kaart>();
    }

    public bool HeeftNogKaarten
    {
        get { return Kaarten.Count > 0; }
    }

    public Kaart LegKaart()
    {
        if (!HeeftNogKaarten)
            return null;

        Kaart kaart = Kaarten[0];
        Kaarten.RemoveAt(0);
        return kaart;
    }
}