using Contracts.Models;

namespace Contracts.Interfaces;

public interface IWordService
{
    Task WriteWordsToDb();
    Task<List<WordModel>> GetWordsList();
    Task WriteWordToCache(WordModel wordModel, IEnumerable<AnagramModel> anagrams);
    Task<CachedWordModel> GetWordFromCache(WordModel wordModel);
    Task ClearCache();
    Task AddSearch(string userIp, string searchString, List<AnagramModel> foundAnagrams);
    Task<IEnumerable<WordModel>> SearchWords(string input);
    Task<Dictionary<string, List<AnagramModel>>> GetSortedWords();
    string Alphabetize(string input);
    string[] ValidateInputWords(string input);
    List<AnagramModel> RemoveDuplicates(List<AnagramModel> anagrams, AnagramModel userInput);
    Task<bool> AddWordToFile(string word);
}