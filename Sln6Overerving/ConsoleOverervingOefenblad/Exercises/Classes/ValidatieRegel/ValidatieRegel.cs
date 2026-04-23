namespace ConsoleOverervingOefenblad.Exercises.Classes.ValidatieRegel;

internal abstract class ValidatieRegel
{
    public abstract bool IsGeldig(string waarde);
    public abstract string FoutBoodschap { get; }
}