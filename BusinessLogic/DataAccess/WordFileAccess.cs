using Contracts.Models;
using Contracts.Interfaces;

namespace BusinessLogic.DataAccess;

public class WordFileAccess : IWordRepository
{
    private const string DictionaryFileName = "zodynas.txt";
    private const string WordCacheFileName = "wordsFileCache.txt";
    private const string SearchHistoryFileName = "searchHistory.txt";

    public async Task<IEnumerable<WordModel>> ReadWords()
    {
        var words = new List<WordModel>();
        await using var fs = File.Open(DictionaryFileName, FileMode.Open, FileAccess.Read);
        await using var bs = new BufferedStream(fs);
        using var sr = new StreamReader(bs);
        string line;
        while ((line = await sr.ReadLineAsync()) != null)
        {
            var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
            var word = new WordModel(parts[0]);
            words.Add(word);
        }

        return words;
    }

    public async Task WriteWord(WordModel wordModel)
    {
        await using var fs = File.Open(DictionaryFileName, FileMode.Append, FileAccess.Write);
        await using var bs = new BufferedStream(fs);
        await using var sw = new StreamWriter(bs);
        await sw.WriteLineAsync(wordModel.Value);
    }

    public async Task WriteWords(IEnumerable<WordModel> words)
    {
        await using var fs = File.Open(DictionaryFileName, FileMode.Append, FileAccess.Write);
        await using var bs = new BufferedStream(fs);
        await using var sw = new StreamWriter(bs);
        foreach (var word in words)
        {
            await sw.WriteLineAsync(word.Value);
        }
    }

    public async Task AddToCache(WordModel wordModel, string anagrams)
    {
        await using var fs = File.Open(WordCacheFileName, FileMode.Append, FileAccess.Write);
        await using var bs = new BufferedStream(fs);
        await using var sw = new StreamWriter(bs);
        await sw.WriteLineAsync(wordModel.Value + '\t' + anagrams);
    }
    
    public async Task<IEnumerable<CachedWordModel>> ReadWordsFromCache()
    {
        var words = new List<CachedWordModel>();
        
        await using var fs = File.Open(WordCacheFileName, FileMode.Open, FileAccess.Read);
        await using var bs = new BufferedStream(fs);
        using var sr = new StreamReader(bs);
        string line;
        while ((line = await sr.ReadLineAsync()) != null)
        {
            var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
            var word = new CachedWordModel(parts[0], parts[1]);
            words.Add(word);
        }

        return words;
    }

    public async Task RemoveWordFromCache(WordModel wordModel)
    {
        var cachedWords = await ReadWordsFromCache();
        cachedWords = cachedWords.Where(x => x.Word != wordModel.Value);
        await using var fs = File.Open(WordCacheFileName, FileMode.Create, FileAccess.Write);
        await using var bs = new BufferedStream(fs);
        await using var sw = new StreamWriter(bs);
        foreach (var cachedWord in cachedWords)
        {
            await sw.WriteLineAsync(cachedWord.Word + '\t' + cachedWord.Anagrams);
        }
    }

    public async Task AddToSearchHistory(SearchHistoryModel model)
    {
        await using var fs = File.Open(SearchHistoryFileName, FileMode.Append, FileAccess.Write);
        await using var bs = new BufferedStream(fs);
        await using var sw = new StreamWriter(bs);
        await sw.WriteLineAsync(model.UserIP + '\t' + model.SearchDate + '\t' + model.SearchString + '\t' + model.FoundAnagrams);
    }

    public async Task<IEnumerable<WordModel>> SearchWordsByFilter(string input)
    {
        var words = new List<WordModel>();
        await using var fs = File.Open(DictionaryFileName, FileMode.Open, FileAccess.Read);
        await using var bs = new BufferedStream(fs);
        using var sr = new StreamReader(bs);
        string line;
        while ((line = await sr.ReadLineAsync()) != null)
        {
            var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
            if (parts[0].Contains(input))
            {
                words.Add(new WordModel(parts[0]));
            }
        }

        return words;
    }
}