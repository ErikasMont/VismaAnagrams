using BusinessLogic.Helpers;
using Contracts.Interfaces;
using Contracts.Models;

namespace BusinessLogic.Services;

public class WordService : IWordService
{
    private readonly IWordRepository _wordFileAccess;
    private readonly IWordRepository _wordDbAccess;

    public WordService(ServiceResolver serviceAccessor)
    {
        _wordFileAccess = serviceAccessor(RepositoryType.File);
        _wordDbAccess = serviceAccessor(RepositoryType.Db);
    }

    public async Task WriteWordsToDb()
    {
        var words = await _wordFileAccess.ReadWords();
        await _wordDbAccess.WriteWords(words);
    }

    public async Task<List<Word>> GetWordsList()
    {
        var words = await _wordDbAccess.ReadWords();
        return words.Distinct().ToList();
    }
    public async Task<Dictionary<string, List<Anagram>>> GetSortedWords()
    {
        var allWords = await _wordFileAccess.ReadWords();
        var words = allWords.ToList();
        
        var sortedDictionary = new Dictionary<string, List<Anagram>>();
        if (words.Count == 0)
        {
            return sortedDictionary;
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
    
    public List<Anagram> RemoveDuplicates(List<Anagram> anagrams, Anagram userInput)
    {
        if (!anagrams.Any())
        {
            return anagrams;
        }
        
        var filteredAnagrams = anagrams.Distinct().ToList();
        
        if (filteredAnagrams.Contains(userInput))
        {
            filteredAnagrams.Remove(userInput);
        }
        
        return filteredAnagrams;
    }

    public async Task<bool> AddWordToFile(string word)
    {
        var exists = await WordExists(word);
        if (exists)
        {
            return false;
        }
        await _wordFileAccess.WriteWord(new Word(word));
        
        return true;
    }

    private async Task<bool> WordExists(string word)
    {
        var words = await _wordFileAccess.ReadWords();
        return words.Any(x => x.Value == word);
    }
}