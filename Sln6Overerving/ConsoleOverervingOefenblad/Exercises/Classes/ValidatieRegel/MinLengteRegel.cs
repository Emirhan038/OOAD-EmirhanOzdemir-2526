namespace ConsoleOverervingOefenblad.Exercises.Classes.ValidatieRegel;

internal class MinLengteRegel : ValidatieRegel
{
    public int MinLengte { get; private set; }

    public MinLengteRegel(int minLengte)
    {
        MinLengte = minLengte;
    }

    public override bool IsGeldig(string waarde) => waarde.Length >= MinLengte;

    public override string FoutBoodschap => $"Waarde moet minstens {MinLengte} tekens bevatten.";
}