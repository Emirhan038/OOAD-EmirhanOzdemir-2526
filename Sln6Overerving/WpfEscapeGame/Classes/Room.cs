namespace WpfEscapeGame.Classes;

public class Room : Actor
{
    public List<Item> Items { get; set; }

    public Room(string name, string desc) : base(name, desc)
    {
        Items = new List<Item>();
    }
}
