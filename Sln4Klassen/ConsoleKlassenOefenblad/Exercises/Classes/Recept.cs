namespace ConsoleKlassenOefenblad.Exercises.Classes;

internal class Recept
{
    public string Titel { get; set; }
    public int Rating { get; set; }
    public bool IsVegetarisch { get; set; } = false;
    public List<string> Ingredienten { get; set; } = new List<string>();
}