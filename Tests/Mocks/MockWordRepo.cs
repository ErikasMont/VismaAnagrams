using Contracts.Interfaces;
using Contracts.Models;

namespace Tests.Mocks;

public class MockWordRepo : IWordRepository
{
    private int _setNumber;

    public MockWordRepo(int setNumber)
    {
        _setNumber = setNumber;
    }
    
    public async Task<IEnumerable<Word>> ReadWords()
    {
        return _setNumber switch
        {
            1 => new List<Word>
            {
                new("rega"), new("visma"), new("kava"), new("alus"),
                new("sula"), new("toli"), new("loti"), new("geras"),
                new("keras"), new("vaga"), new("toli"), new("rimti"),
                new("kilti"), new("likti"), new("mirti"), new("loti"),
                new("kiloti"), new("ly"), new("tirti"), new("irti"),
                new("kloti"), new("lyti"), new("tiras"), new("rasit"),
                new("rasti"), new("labas"), new("rytas"), new("balas"),
                new("tyras"), new("baslys"), new("tara"), new("ryti")
            },
            2 => new List<Word>()
        };

    }

    /// <summary>
    /// Method required for interface realization
    /// </summary>
    /// <param name="word"></param>
    public async Task WriteWord(Word word)
    {
       
    }

    /// <summary>
    /// Method required for interface realization
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task WriteWords(IEnumerable<Word> words)
    {
        
    }

    /// <summary>
    /// Method required for interface realization
    /// </summary>
    /// <param name="word"></param>
    /// <param name="anagrams"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task AddToCache(Word word, string anagrams)
    {
        
    }
    
    /// <summary>
    /// Method required for interface realization
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<CachedWord>> ReadWordsFromCache()
    {
        return _setNumber switch
        {
            1 => new List<CachedWord>
            {
                new("rega", "gera"), new("labas", "balas"), new("rytas", "tyras"),
                new("alus", "sula")
            },
            2 => new List<CachedWord>()
        };
    }

    /// <summary>
    /// Method required for interface realization
    /// </summary>
    /// <param name="word"></param>
    public async Task RemoveWordFromCache(Word word)
    {
        
    }

    /// <summary>
    /// Method required for interface realization
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task AddToSearchHistory(SearchHistory model)
    {
        
    }

    /// <summary>
    /// Method required for interface realization
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Word>> SearchWordsByFilter(string input)
    {
        return new List<Word>
        {
            new("rega"), new("visma"), new("kava"), new("alus"),
            new("sula"), new("toli"), new("loti"), new("geras"),
            new("keras"), new("vaga"), new("toli"), new("rimti"),
            new("kilti"), new("likti"), new("mirti"), new("loti"),
            new("kiloti"), new("ly"), new("tirti"), new("irti"),
            new("kloti"), new("lyti"), new("tiras"), new("rasit"),
            new("rasti"), new("labas"), new("rytas"), new("balas"),
            new("tyras"), new("baslys"), new("tara"), new("ryti")
        };
    }
}