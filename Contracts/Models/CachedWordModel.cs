namespace Contracts.Models;

public class CachedWordModel
{
    public string Word { get; set; }
    public string Anagrams { get; set; }

    public CachedWordModel(string word, string anagrams)
    {
        Word = word;
        Anagrams = anagrams;
    }
}