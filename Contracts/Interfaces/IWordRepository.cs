using Contracts.Models;

namespace Contracts.Interfaces;

public interface IWordRepository
{
     Task<IEnumerable<WordModel>> ReadWords();
     Task WriteWord(WordModel wordModel);
     Task WriteWords(IEnumerable<WordModel> words);
     Task AddToCache(WordModel wordModel, string anagrams);
     Task<IEnumerable<CachedWordModel>> ReadWordsFromCache();
     Task RemoveWordFromCache(WordModel wordModel);
     Task AddToSearchHistory(SearchHistoryModel model);
     Task<IEnumerable<WordModel>> SearchWordsByFilter(string input);
}