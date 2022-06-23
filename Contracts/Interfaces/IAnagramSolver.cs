using Contracts.Models;

namespace Contracts.Interfaces;

public interface IAnagramSolver
{
    Task<List<Anagram>> GetAnagrams(string myWords, int numberOfAnagrams);
}