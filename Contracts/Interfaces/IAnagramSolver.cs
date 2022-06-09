using Contracts.Models;

namespace Contracts.Interfaces;

public interface IAnagramSolver
{
    List<Anagram> GetAnagrams(string myWords, int numberOfAnagrams);
}