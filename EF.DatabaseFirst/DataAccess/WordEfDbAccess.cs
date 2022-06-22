using System.Data;
using Contracts.Interfaces;
using Contracts.Models;
using EF.DatabaseFirst.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EF.DatabaseFirst.DataAccess;

public class WordEfDbAccess : IWordRepository
{
    private readonly AnagramsDbContext _context;

    public WordEfDbAccess(AnagramsDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<WordModel>> ReadWords()
    {
        return await _context.Words.Select(x => new WordModel(x.Value)).ToListAsync();
    }

    public async Task WriteWord(WordModel wordModel)
    {
        await _context.Words.AddAsync(new Word(wordModel.Value));
        await _context.SaveChangesAsync();
    }

    public async Task WriteWords(IEnumerable<WordModel> words)
    {
        foreach (var word in words)
        {
            await _context.Words.AddAsync(new Word(word.Value));
        }
        await _context.SaveChangesAsync();
    }

    public async Task AddToCache(WordModel wordModel, string anagrams)
    {
        await _context.CachedWords.AddAsync(new CachedWord(wordModel.Value, anagrams));
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CachedWordModel>> ReadWordsFromCache()
    {
        return await _context.CachedWords.Select(x => new CachedWordModel(x.Word, x.Anagrams)).ToListAsync();
    }

    public async Task RemoveWordFromCache(WordModel wordModel)
    {
        var givenWord = new SqlParameter("@GivenWord", SqlDbType.VarChar, 50)
        {
            Value = wordModel.Value
        };
        await _context.Database.ExecuteSqlRawAsync("exec spRemoveCachedWord @GivenWord", givenWord);
        await _context.SaveChangesAsync();
    }

    public async Task AddToSearchHistory(SearchHistoryModel model)
    {
        await _context.SearchHistories.AddAsync(
            new SearchHistory(model.UserIP, model.SearchDate, model.SearchString, model.FoundAnagrams));
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<WordModel>> SearchWordsByFilter(string input)
    {
        return await _context.Words.Where(x => x.Value.Contains(input))
            .Select(x => new WordModel(x.Value)).ToListAsync();
    }
}