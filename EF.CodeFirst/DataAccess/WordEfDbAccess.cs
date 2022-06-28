using System.Data;
using Contracts.Interfaces;
using EF.CodeFirst.Data;
using EF.CodeFirst.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EF.CodeFirst.DataAccess;

public class WordEfDbAccess : IWordRepository
{
    private readonly AnagramsCodeFirstDbContext _context;

    public WordEfDbAccess(AnagramsCodeFirstDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Contracts.Models.Word>> ReadWords()
    {
        return await _context.Words.Select(x => new Contracts.Models.Word(x.Value)).ToListAsync();
    }

    public async Task WriteWord(Contracts.Models.Word word)
    {
        await _context.Words.AddAsync(new Models.Word(){ Value = word.Value});
    }

    public async Task WriteWords(IEnumerable<Contracts.Models.Word> words)
    {
        foreach (var word in words)
        {
            await _context.Words.AddAsync(new Models.Word(){Value = word.Value});
        }
    }

    public async Task AddToCache(Contracts.Models.Word word, string anagrams)
    {
        var foundWord = await _context.Words.FirstOrDefaultAsync(x => x.Value == word.Value);
        await _context.CachedWords.AddAsync(new Models.CachedWord(){ Anagrams = anagrams, WordId = foundWord.Id});
    }

    public async Task<IEnumerable<Contracts.Models.CachedWord>> ReadWordsFromCache()
    {
        return await _context.CachedWords.Select(x =>
            new Contracts.Models.CachedWord(x.Word.Value, x.Anagrams)).ToListAsync();
    }

    public async Task RemoveWordFromCache(Contracts.Models.Word word)
    {
        var givenWord = new SqlParameter("@GivenWord", SqlDbType.VarChar, 50)
        {
            Value = word.Value
        };
        await _context.Database.ExecuteSqlRawAsync("exec spRemoveCachedWord @GivenWord", givenWord);
    }

    public async Task AddToSearchHistory(Contracts.Models.SearchHistory model)
    {
        await _context.SearchHistories.AddAsync(
            new Models.SearchHistory(){UserIp = model.UserIP, SearchDate = model.SearchDate, 
                SearchString = model.SearchString, FoundAnagrams = model.FoundAnagrams});
    }

    public async Task<IEnumerable<Contracts.Models.Word>> SearchWordsByFilter(string input)
    {
        return await _context.Words.Where(x => x.Value.Contains(input))
            .Select(x => new Contracts.Models.Word(x.Value)).ToListAsync();
    }

    public async Task RemoveWord(string word)
    {
        var foundWord = await _context.Words.FirstOrDefaultAsync(x => x.Value == word);
        _context.Words.Remove(foundWord);
    }

    public async Task EditWord(string existingWord, string editedWord)
    {
        var word = await _context.Words.FirstOrDefaultAsync(x => x.Value == existingWord);
        word.Value = editedWord;
        _context.Words.Update(word);
    }

    public async Task Commit()
    {
        await _context.SaveChangesAsync();
    }
}