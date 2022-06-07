using Contracts.Models;

namespace Contracts.Interfaces;

public interface IWordRepository
{
    public Dictionary<string, List<Anagram>> ReadDictionary();
}