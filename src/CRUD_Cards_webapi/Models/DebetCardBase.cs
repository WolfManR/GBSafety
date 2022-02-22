namespace CRUD_Cards_webapi.Models;

internal class DebetCardBase
{
    public string Number { get; init; }
    public string Holder { get; init; }
    public int ExpireMonth { get; init; }
    public int ExpireYear { get; init; }
}