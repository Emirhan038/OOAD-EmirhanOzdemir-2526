namespace ConsoleKlassenOefenblad.Exercises.Classes;

internal class Product
{
    public int ProductId { get; set; }
    public string Naam { get; set; }
    public string Beschrijving { get; set; }
    public decimal Prijs { get; set; }
    public int Voorraad { get; set; }
    public bool IsInVoorraad { get { return Voorraad > 0; } }
    public double Korting
    {
        get;
        set
        {
            if (value < 0 || value > 100)
            {
                throw new ArgumentException("Percentage moet tussen 0 en 100 liggen.");
            }
            field = value;
        }
    } = 0;

    public decimal PrijsMetKorting
    {
        get
        {
            return Prijs * (1 - ((decimal)Korting) / 100);
        }
    }

    public override string ToString()
    {
        string voorraad = IsInVoorraad ? "in voorraad" : "niet in voorraad";
        return $"[{ProductId}] {Naam} – € {PrijsMetKorting:F2} | {voorraad}";
    }
}