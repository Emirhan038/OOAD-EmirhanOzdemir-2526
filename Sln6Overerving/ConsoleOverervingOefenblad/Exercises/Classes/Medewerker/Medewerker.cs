namespace ConsoleOverervingOefenblad.Exercises.Classes.Medewerker;

internal class Medewerker
{
    public string Naam { get; set; }
    public string Afdeling { get; set; }

    public Medewerker(string naam, string afdeling)
    {
        Naam = naam;
        Afdeling = afdeling;
    }

    public override string ToString()
    {
        return $"{Naam} ({Afdeling})";
    }
}
