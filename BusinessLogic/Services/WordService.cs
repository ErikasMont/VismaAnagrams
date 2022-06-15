using Contracts.Interfaces;
using Contracts.Models;

namespace BusinessLogic.Services;

public class WordService : IWordService
{
    private readonly IWordRepository _wordRepository;

    public WordService(IWordRepository wordRepository)
    {
        _wordRepository = wordRepository;
    }
    public Dictionary<string, List<Anagram>>? GetSortedWords()
    {
        var words = _wordRepository.ReadWords();
        var sortedDictionary = new Dictionary<string, List<Anagram>>();
        if (words.Count == 0)
        {
            return null;
        }
        
        foreach (var word in words)
        {
            var anagram = new Anagram(word.Value);
            var sortedKey = Alphabetize(word.Value);

            if (sortedDictionary.ContainsKey(sortedKey))
            {
                sortedDictionary[sortedKey].Add(anagram);
            }
            else
            {
                var anagrams = new List<Anagram> { anagram };
                sortedDictionary.Add(sortedKey, anagrams);
            }
        }
        
        return sortedDictionary;
    }
    
    public string Alphabetize(string value)
    {
        return string.Concat(value.OrderBy(v => v));
    }

    public string[] ValidateInputWords(string input)
    {
        var parts = input.Split(' ');
        if (parts.Contains(""))
        {
            parts = parts.Where(x => x != "").ToArray();
        }

        return parts;
    }
    
    public List<Anagram>? RemoveDuplicates(List<Anagram> anagrams, Anagram userInput)
    {
        if (!anagrams.Any())
        {
            return null;
        }
        
        var filteredAnagrams = anagrams.Distinct().ToList();
        
        if (filteredAnagrams.Contains(userInput))
        {
            filteredAnagrams.Remove(userInput);
        }
        
        return filteredAnagrams;
    }

    public bool AddWordToFile(string word)
    {
        var words = _wordRepository.ReadWords();
        if (words.Any(x => x.Value == word))
        {
            return false;
        }
        
        words.Add(new Word(word));
        words = words.OrderBy(x => x.Value).ToList();
        _wordRepository.WriteWords(words);
        
        return true;
    }
}