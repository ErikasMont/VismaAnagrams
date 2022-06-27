namespace Contracts.Models;

public class CachedWord
{
    public string Word { get; set; }
    public string Anagrams { get; set; }

    public CachedWord(string word, string anagrams)
    {
        Word = word;
        Anagrams = anagrams;
    }
}