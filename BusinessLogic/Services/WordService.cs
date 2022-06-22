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

    public async Task<List<WordModel>> GetWordsList()
    {
        var words = await _wordDbAccess.ReadWords();
        return words.Distinct().ToList();
    }

    public async Task WriteWordToCache(WordModel wordModel, IEnumerable<AnagramModel> anagrams)
    {
        var wordAnagrams = anagrams.Aggregate("", (current, anagram) => current + (anagram.Word + " "));
        await _wordDbAccess.AddToCache(wordModel, wordAnagrams);
    }

    public async Task<CachedWordModel> GetWordFromCache(WordModel wordModel)
    {
        var cachedWords = await _wordDbAccess.ReadWordsFromCache();
        var cachedWord = cachedWords.Where(x => x.Word == wordModel.Value).ToList();
        return cachedWord.Count == 0 ? null : cachedWord[0];
    }

    public async Task ClearCache()
    {
        var cachedWords = await _wordDbAccess.ReadWordsFromCache();
        foreach (var cachedWord in cachedWords)
        {
            await _wordDbAccess.RemoveWordFromCache(new WordModel(cachedWord.Word));
        }
    }

    public async Task AddSearch(string userIp, string searchString, List<AnagramModel> foundAnagrams)
    {
        var anagrams = foundAnagrams == null ? ""
            : foundAnagrams.Aggregate("", (current, anagram) => current + (anagram.Word + " "));
        var searchHistoryModel = new SearchHistoryModel(userIp, DateTime.Now, searchString, anagrams);
        await _wordDbAccess.AddToSearchHistory(searchHistoryModel);
    }

    public async Task<IEnumerable<WordModel>> SearchWords(string input)
    {
        return await _wordDbAccess.SearchWordsByFilter(input);
    } 
    
    public async Task<Dictionary<string, List<AnagramModel>>> GetSortedWords()
    {
        var allWords = await _wordFileAccess.ReadWords();
        var words = allWords.ToList();
        
        var sortedDictionary = new Dictionary<string, List<AnagramModel>>();
        if (words.Count == 0)
        {
            return sortedDictionary;
        }
        
        foreach (var word in words)
        {
            var anagram = new AnagramModel(word.Value);
            var sortedKey = Alphabetize(word.Value);

            if (sortedDictionary.ContainsKey(sortedKey))
            {
                sortedDictionary[sortedKey].Add(anagram);
            }
            else
            {
                var anagrams = new List<AnagramModel> { anagram };
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
    
    public List<AnagramModel> RemoveDuplicates(List<AnagramModel> anagrams, AnagramModel userInput)
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
        await _wordFileAccess.WriteWord(new WordModel(word));
        
        return true;
    }

    private async Task<bool> WordExists(string word)
    {
        var words = await _wordFileAccess.ReadWords();
        return words.Any(x => x.Value == word);
    }
}