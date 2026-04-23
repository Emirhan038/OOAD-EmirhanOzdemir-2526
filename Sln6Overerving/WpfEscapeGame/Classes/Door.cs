namespace WpfEscapeGame.Classes;

public class Door : Actor
{
    public bool IsLocked { get; set; }
    public Item? Key { get; set; }
    public Item? HiddenItem { get; set; }
    public Room? ToRoom { get; set; }

    public Door(string name, string desc) : base(name, desc)
    {
    }
}
