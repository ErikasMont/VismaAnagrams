using Contracts.Models;

namespace Contracts.Interfaces;

public interface IWordRepository
{
     Task<IEnumerable<Word>> ReadWords();
     Task WriteWord(Word word);
     Task WriteWords(IEnumerable<Word> words);
}