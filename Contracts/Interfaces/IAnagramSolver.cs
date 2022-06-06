using Contracts.Models;

namespace Contracts.Interfaces;

public interface IAnagramSolver
{
    public List<Anagram> GetAnagrams(string myWords, int anagramsCount);
}