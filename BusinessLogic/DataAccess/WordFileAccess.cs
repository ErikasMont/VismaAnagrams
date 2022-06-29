using Contracts.Models;
using Contracts.Interfaces;

namespace BusinessLogic.DataAccess;

public class WordFileAccess : IWordRepository
{
    private const string DictionaryFileName = "zodynas.txt";
    private const string WordCacheFileName = "wordsFileCache.txt";
    private const string SearchHistoryFileName = "searchHistory.txt";

    public async Task<IEnumerable<Word>> ReadWords()
    {
        var words = new List<Word>();
        await using var fs = File.Open(DictionaryFileName, FileMode.Open, FileAccess.Read);
        await using var bs = new BufferedStream(fs);
        using var sr = new StreamReader(bs);
        string line;
        while ((line = await sr.ReadLineAsync()) != null)
        {
            var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
            var word = new Word(parts[0]);
            words.Add(word);
        }

        return words;
    }

    public async Task WriteWord(Word word)
    {
        await using var fs = File.Open(DictionaryFileName, FileMode.Append, FileAccess.Write);
        await using var bs = new BufferedStream(fs);
        await using var sw = new StreamWriter(bs);
        await sw.WriteLineAsync(word.Value);
    }

    public async Task WriteWords(IEnumerable<Word> words)
    {
        await using var fs = File.Open(DictionaryFileName, FileMode.Append, FileAccess.Write);
        await using var bs = new BufferedStream(fs);
        await using var sw = new StreamWriter(bs);
        foreach (var word in words)
        {
            await sw.WriteLineAsync(word.Value);
        }
    }

    public async Task AddToCache(Word word, string anagrams)
    {
        await using var fs = File.Open(WordCacheFileName, FileMode.Append, FileAccess.Write);
        await using var bs = new BufferedStream(fs);
        await using var sw = new StreamWriter(bs);
        await sw.WriteLineAsync(word.Value + '\t' + anagrams);
    }
    
    public async Task<IEnumerable<CachedWord>> ReadWordsFromCache()
    {
        var words = new List<CachedWord>();
        
        await using var fs = File.Open(WordCacheFileName, FileMode.Open, FileAccess.Read);
        await using var bs = new BufferedStream(fs);
        using var sr = new StreamReader(bs);
        string line;
        while ((line = await sr.ReadLineAsync()) != null)
        {
            var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
            var word = new CachedWord(parts[0], parts[1]);
            words.Add(word);
        }

        return words;
    }

    public async Task RemoveWordFromCache(Word word)
    {
        var cachedWords = await ReadWordsFromCache();
        cachedWords = cachedWords.Where(x => x.Word != word.Value);
        await using var fs = File.Open(WordCacheFileName, FileMode.Create, FileAccess.Write);
        await using var bs = new BufferedStream(fs);
        await using var sw = new StreamWriter(bs);
        foreach (var cachedWord in cachedWords)
        {
            await sw.WriteLineAsync(cachedWord.Word + '\t' + cachedWord.Anagrams);
        }
    }

    public async Task AddToSearchHistory(SearchHistory model)
    {
        await using var fs = File.Open(SearchHistoryFileName, FileMode.Append, FileAccess.Write);
        await using var bs = new BufferedStream(fs);
        await using var sw = new StreamWriter(bs);
        await sw.WriteLineAsync(model.UserIP + '\t' + model.SearchDate + '\t' + model.SearchString + '\t' + model.FoundAnagrams);
    }

    public async Task<IEnumerable<Word>> SearchWordsByFilter(string input)
    {
        var words = new List<Word>();
        await using var fs = File.Open(DictionaryFileName, FileMode.Open, FileAccess.Read);
        await using var bs = new BufferedStream(fs);
        using var sr = new StreamReader(bs);
        string line;
        while ((line = await sr.ReadLineAsync()) != null)
        {
            var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
            if (parts[0].Contains(input))
            {
                words.Add(new Word(parts[0]));
            }
        }

        return words;
    }

    public Task RemoveWord(string word)
    {
        throw new NotImplementedException();
    }

    public Task EditWord(string existingWord, string editedWord)
    {
        throw new NotImplementedException();
    }

    public Task Commit()
    {
        throw new NotImplementedException();
    }
}