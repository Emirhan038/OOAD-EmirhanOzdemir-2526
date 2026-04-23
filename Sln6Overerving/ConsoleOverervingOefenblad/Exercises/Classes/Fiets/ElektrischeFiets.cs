namespace ConsoleOverervingOefenblad.Exercises.Classes.Fiets;

internal class ElektrischeFiets : Fiets
{
    // verplichte properties
    public int Kwh { get; set; }

    // optionele properties
    public double? BatterijGewicht { get; set; } = null;

    public ElektrischeFiets(string merk, int versnellingen, FietsMateriaalType materiaal, int kwh) : base(merk, versnellingen, materiaal)
    {
        Kwh = kwh;
    }

    public override string ToString()
    {
        string str = base.ToString();
        if (BatterijGewicht != null) str += $", batterijgewicht {BatterijGewicht:F1}kg";
        return $"{str}, batterij {Kwh}kWh";
    }
}
