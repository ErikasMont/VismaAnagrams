namespace EF.CodeFirst.Models;

public class Word
{
    public int Id { get; set; }
    public string Value { get; set; }
    public CachedWord CachedWord { get; set; }
}