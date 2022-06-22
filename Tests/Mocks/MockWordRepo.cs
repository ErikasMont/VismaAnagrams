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
    
    public async Task<IEnumerable<WordModel>> ReadWords()
    {
        return _setNumber switch
        {
            1 => new List<WordModel>
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
            2 => new List<WordModel>()
        };

    }

    /// <summary>
    /// Method required for interface realization
    /// </summary>
    /// <param name="wordModel"></param>
    public async Task WriteWord(WordModel wordModel)
    {
       
    }

    /// <summary>
    /// Method required for interface realization
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task WriteWords(IEnumerable<WordModel> words)
    {
        
    }

    /// <summary>
    /// Method required for interface realization
    /// </summary>
    /// <param name="wordModel"></param>
    /// <param name="anagrams"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task AddToCache(WordModel wordModel, string anagrams)
    {
        
    }
    
    /// <summary>
    /// Method required for interface realization
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<CachedWordModel>> ReadWordsFromCache()
    {
        return _setNumber switch
        {
            1 => new List<CachedWordModel>
            {
                new("rega", "gera"), new("labas", "balas"), new("rytas", "tyras"),
                new("alus", "sula")
            },
            2 => new List<CachedWordModel>()
        };
    }

    /// <summary>
    /// Method required for interface realization
    /// </summary>
    /// <param name="wordModel"></param>
    public async Task RemoveWordFromCache(WordModel wordModel)
    {
        
    }

    /// <summary>
    /// Method required for interface realization
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task AddToSearchHistory(SearchHistoryModel model)
    {
        
    }

    /// <summary>
    /// Method required for interface realization
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<IEnumerable<WordModel>> SearchWordsByFilter(string input)
    {
        return new List<WordModel>
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