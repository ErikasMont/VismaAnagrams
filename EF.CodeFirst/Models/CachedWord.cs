namespace EF.CodeFirst.Models;

public class CachedWord
{
    public int Id { get; set; }
    public string? Anagrams { get; set; }
    public int WordId { get; set; }
    public Word Word { get; set; }
}