using Contracts.Models;

namespace Contracts.Interfaces;

public interface IWordService
{
    Task WriteWordsToDb();
    Task<List<Word>> GetWordsList();
    Task<Dictionary<string, List<Anagram>>?> GetSortedWords();
    string Alphabetize(string input);
    string[] ValidateInputWords(string input);
    List<Anagram>? RemoveDuplicates(List<Anagram> anagrams, Anagram userInput);
    Task<bool> AddWordToFile(string word);
}