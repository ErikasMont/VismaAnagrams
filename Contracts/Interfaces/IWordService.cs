using Contracts.Models;

namespace Contracts.Interfaces;

public interface IWordService
{
    void TransferWords();
    Dictionary<string, List<Anagram>>? GetSortedWords();
    string Alphabetize(string input);
    string[] ValidateInputWords(string input);
    List<Anagram>? RemoveDuplicates(List<Anagram> anagrams, Anagram userInput);
    bool AddWordToFile(string word);
    bool WordExists(string word);
}