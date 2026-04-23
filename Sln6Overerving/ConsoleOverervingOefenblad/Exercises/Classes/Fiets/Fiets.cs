namespace ConsoleOverervingOefenblad.Exercises.Classes.Fiets;

internal class Fiets
{
    // verplichte properties
    public string Merk { get; set; }
    public int Versnellingen { get; set; }
    public FietsMateriaalType Materiaal { get; set; }

    // optionele properties
    public int? LichaamsLengte { get; set; } = null;
    public double? Gewicht { get; set; } = null;
    public FietsKleur? Kleur { get; set; } = null;
    public string? Beschrijving { get; set; } = null;

    // constructor met verplichte properties
    public Fiets(string merk, int versnellingen, FietsMateriaalType materiaal)
    {
        Merk = merk;
        Versnellingen = versnellingen;
        Materiaal = materiaal;
    }

    // override ToString()
    public override string ToString()
    {
        string str = $"{Merk}: {Versnellingen} versnellingen, materiaal {Materiaal}";
        if (LichaamsLengte != null) str += $", lichaamslengte {LichaamsLengte}cm";
        if (Gewicht != null) str += $", gewicht {Gewicht:F1}kg";
        if (Kleur != null) str += $", kleur: {Kleur}";
        return str;
    }
}
