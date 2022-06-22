using Contracts.Models;

namespace Contracts.Interfaces;

public interface IAnagramSolver
{
    Task<List<AnagramModel>> GetAnagrams(string myWords, int numberOfAnagrams);
}