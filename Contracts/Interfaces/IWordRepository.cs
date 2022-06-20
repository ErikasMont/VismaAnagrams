using Contracts.Models;

namespace Contracts.Interfaces;

public interface IWordRepository
{
     Task<IEnumerable<Word>> ReadWords();
     Task WriteWord(Word word);
     Task WriteWords(IEnumerable<Word> words);
     Task AddToCache(Word word, string anagrams);
     Task<IEnumerable<CachedWordModel>> ReadWordsFromCache();
     Task RemoveWordFromCache(Word word);
     Task AddToSearchHistory(SearchHistoryModel model);
     Task<IEnumerable<Word>> SearchWordsByFilter(string input);
}