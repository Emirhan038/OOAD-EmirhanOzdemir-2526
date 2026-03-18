public class Kaart
{
    public string Kleur { get; set; }
    public int Nummer { get; set; }

    public Kaart(string kleur, int nummer)
    {
        Kleur = kleur;
        Nummer = nummer;
    }
}