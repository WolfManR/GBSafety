namespace CRUD_Cards_webapi.Entities;

internal sealed class DebetCardEntity
{
    public int Id { get; init; }
    public string Number { get; set; }
    public string Holder { get; set; }
    public int ExpireMonth { get; set; }
    public int ExpireYear { get; set; }
}