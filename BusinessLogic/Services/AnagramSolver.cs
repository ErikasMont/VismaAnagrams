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
        var anagrams = new List<Anagram>();
        var anagram = new Anagram(myWords);
        var words = _wordRepository.ReadWords();
        
        char[] letters = myWords.ToCharArray();
        Array.Sort(letters);
        var key = new string(letters);

        var sortedWords = SortWords(words);

        if (sortedWords.TryGetValue(key, out anagrams))
        {
            if (anagrams.Count == 0)
            {
                return anagrams;
            }
            
            var filteredAnagrams = RemoveDuplicates(anagrams, anagram);
            
            if (filteredAnagrams.Count == 0)
            {
                return filteredAnagrams;
            }
            else
            {
                return filteredAnagrams.GetRange(0, anagramsCount);
            }
        }
        
        return anagrams;
    }

    private Dictionary<string, List<Anagram>> SortWords(List<Word> words)
    {
        var sortedDictionary = new Dictionary<string, List<Anagram>>();
        foreach (var word in words)
        {
            var anagram = new Anagram(word.Value);
            char[] letters = word.Value.ToCharArray();
            Array.Sort(letters);
            var sortedKey = new string(letters);

            if (sortedDictionary.ContainsKey(sortedKey))
            {
                sortedDictionary[sortedKey].Add(anagram);
            }
            else
            {
                var anagrams = new List<Anagram>();
                anagrams.Add(anagram);
                sortedDictionary.Add(sortedKey, anagrams);
            }
        }
        
        return sortedDictionary;
    }

    private List<Anagram> RemoveDuplicates(List<Anagram> anagrams, Anagram userAnagram)
    {
        var filteredAnagrams = anagrams.Distinct().ToList();
        
        if (filteredAnagrams.Contains(userAnagram))
        {
            filteredAnagrams.Remove(userAnagram);
        }
        
        return filteredAnagrams;
    }
}