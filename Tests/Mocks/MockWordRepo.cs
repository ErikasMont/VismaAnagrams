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
    
    public List<Word> ReadWords()
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
    /// <param name="words"></param>
    public void WriteWords(List<Word> words)
    {
       
    }
}