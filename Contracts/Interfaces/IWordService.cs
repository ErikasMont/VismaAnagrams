using Contracts.Models;

namespace Contracts.Interfaces;

public interface IWordService
{
    Dictionary<string, List<Anagram>>? GetSortedWords();
    string Alphabetize(string input);
    List<Anagram>? RemoveDuplicates(List<Anagram> anagrams, Anagram userInput);
}