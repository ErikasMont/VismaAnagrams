using BusinessLogic.Repositories;
using Contracts.Interfaces;
using Contracts.Models;

namespace BusinessLogic.Services;

public class AnagramSolver : Contracts.Interfaces.IAnagramSolver
{
    private IWordRepository _wordRepository;

    public AnagramSolver(IWordRepository wordRepository)
    {
        _wordRepository = wordRepository;
    }
    public List<Anagram> GetAnagrams(string myWords, int anagramsCount)
    {
        List<Anagram> anagrams = new List<Anagram>();
        Anagram anagram = new Anagram(myWords);
        var dictionary = _wordRepository.ReadDictionary("../../../../zodynas.txt");
        
        char[] letters = myWords.ToCharArray();
        Array.Sort(letters);
        string key = new string(letters);
        
        if (dictionary.TryGetValue(key, out anagrams))
        {
            if (anagrams.Contains(anagram))
            {
                anagrams.Remove(anagram);
            }

            if (anagrams.Count == 0)
            {
                return anagrams;
            }
            else
            {
                return anagrams.GetRange(0, anagramsCount);
            }
        }
        
        return anagrams;
    }
}