using Contracts.Models;

namespace Contracts.Interfaces;

public interface IWordService
{
    Task WriteWordsToDb();
    Task<List<Word>> GetWordsList();
    Task WriteWordToCache(Word word, IEnumerable<Anagram> anagrams);
    Task<CachedWord> GetWordFromCache(Word word);
    Task ClearCache();
    Task AddSearch(string userIp, string searchString, List<Anagram> foundAnagrams);
    Task<IEnumerable<Word>> SearchWords(string input);
    Task<Dictionary<string, List<Anagram>>> GetSortedWords();
    string Alphabetize(string input);
    string[] ValidateInputWords(string input);
    List<Anagram> RemoveDuplicates(List<Anagram> anagrams, Anagram userInput);
    Task<bool> AddWord(string word);
    Task RemoveWord(string word);
    Task<bool> EditWord(string existingWord, string editedWord);
}