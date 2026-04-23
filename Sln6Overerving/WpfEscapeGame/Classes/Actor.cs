namespace WpfEscapeGame.Classes;

public class Actor
{
    public string Name { get; set; }
    public string Description { get; set; }

    public Actor(string name, string desc)
    {
        Name = name;
        Description = desc;
    }

    public override string ToString()
    {
        return $"{Name}: {Description}";
    }
}
