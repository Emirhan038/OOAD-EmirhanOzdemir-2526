namespace WpfLayoutUIOefenblad.Helpers;

public sealed class NavPageInfo
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required int Order { get; init; }
    public required bool IsVisible { get; init; }
    public required Type PageType { get; init; }
}