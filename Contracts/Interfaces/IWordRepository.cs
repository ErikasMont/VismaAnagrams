using Contracts.Models;

namespace Contracts.Interfaces;

public interface IWordRepository
{
     List<Word> ReadWords();
}