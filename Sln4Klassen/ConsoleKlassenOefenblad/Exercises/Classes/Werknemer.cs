namespace ConsoleKlassenOefenblad.Exercises.Classes;

internal class Werknemer
{
    public int Id { get; set; }
    public string Naam { get; set; }

    private decimal _salaris;
    public decimal Salaris
    {
        get { return _salaris; }
        set
        {
            if (value < 0)
                throw new ArgumentException("Salaris kan niet negatief zijn");
            _salaris = value;
        }
    }

    private DateOnly _inDienstSinds;
    public DateOnly InDienstSinds
    {
        get { return _inDienstSinds; }
        set
        {
            if (value > DateOnly.FromDateTime(DateTime.Now))
                throw new ArgumentException("Datum indiensttreding kan niet in de toekomst liggen");
            _inDienstSinds = value;
        }
    }

    public int Ancienniteit => DateOnly.FromDateTime(DateTime.Now).Year - InDienstSinds.Year;

    public string Seniority
    {
        get
        {
            if (Ancienniteit < 2) return "Junior";
            if (Ancienniteit < 5) return "Medior";
            return "Senior";
        }
    }

    public void GeefOpslag(decimal percentage)
    {
        Salaris = Salaris * (1 + percentage / 100);
    }
}