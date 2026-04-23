namespace WpfEscapeGame.Classes;

public class Item : Actor
{
    public bool IsLocked { get; set; }
    public Item? Key { get; set; }
    public Item? HiddenItem { get; set; }
    public bool IsPortable { get; set; }

    public Item(string name, string desc) : base(name, desc)
    {
    }
}
