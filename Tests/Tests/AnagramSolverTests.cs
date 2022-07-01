using BusinessLogic.Helpers;
using BusinessLogic.Services;
using Contracts.Interfaces;
using Contracts.Models;

namespace Tests.Tests;

[TestFixture]
public class AnagramSolverTests
{
    private IAnagramSolver _anagramSolver;
    private IWordRepository _wordRepository;

    [SetUp]
    public void Setup()
    {
        _wordRepository = Substitute.For<IWordRepository>();
        var serviceResolver = new ServiceResolver(key => key switch
        {
            RepositoryType.File => _wordRepository,
            RepositoryType.Db => _wordRepository
        });
        _anagramSolver = new AnagramSolver( new WordService(serviceResolver));
    }

    [TestCase("rega")]
    [TestCase("          ")]
    [TestCase("")]
    [TestCase("visma kava")]
    public async Task GetAnagrams_IfInputWithoutAnagramsGiven_EmptyListOfAnagrams(string input)
    {
        var expectedCount = 0;
        _wordRepository.ReadWords().Returns(new List<Word>()
        {
            new ("alus"), new ("sula")
        });

        var anagrams = await _anagramSolver.GetAnagrams(input, 2);
        
        anagrams.ShouldNotBeNull();
        anagrams.Count.ShouldBe(expectedCount);
        _wordRepository.Received().ReadWords();
    }
    
    [TestCase("alus", "sula")]
    [TestCase("toli  ", "loti")]
    [TestCase("geras kava", "keras vaga")]
    [TestCase("geras   kava", "keras vaga")]
    [TestCase("toli rimti kilti", "loti mirti likti")]
    public async Task GetAnagrams_IfInputWithOneAnagramGiven_ListOfAnagrams(string input, string output)
    {
        var expectedCount = 1;
        var expectedAnagram = new Anagram(output);
        _wordRepository.ReadWords().Returns(new List<Word>()
        {
            new("rega"), new("visma"), new("kava"), new("alus"),
            new("sula"), new("toli"), new("loti"), new("geras"),
            new("keras"), new("vaga"), new("toli"), new("rimti"),
            new("kilti"), new("likti"), new("mirti"), new("loti"),
            new("kiloti"), new("ly"), new("tirti"), new("irti"),
            new("kloti"), new("lyti"), new("tiras"), new("rasit"),
            new("rasti"), new("labas"), new("rytas"), new("balas"),
            new("tyras"), new("baslys"), new("tara"), new("ryti")
        });

        var anagrams = await _anagramSolver.GetAnagrams(input, 2);
        
        anagrams.ShouldNotBeNull();
        anagrams.Count.ShouldBe(expectedCount);
        expectedAnagram.ShouldBeOneOf(anagrams.ToArray());
        _wordRepository.Received().ReadWords();
    }
    
    [TestCase("tiras", new []{"rasit", "rasti"})]
    [TestCase("tiras   ", new []{"rasit", "rasti"})]
    [TestCase("labas rytas", new []{"balas tyras", "baslys tara"})]
    [TestCase("toli ryti kilti", new []{"irti kloti lyti", "kiloti ly tirti"})]
    public async Task GetAnagrams_IfInputWithMultipleAnagramsGiven_ListOfAnagrams(string input, string[] output)
    {
        var expectedCount = 2;
        var expectedAnagrams = output.Select(x => new Anagram(x)).ToArray();
        _wordRepository.ReadWords().Returns(new List<Word>()
        {
            new("rega"), new("visma"), new("kava"), new("alus"),
            new("sula"), new("toli"), new("loti"), new("geras"),
            new("keras"), new("vaga"), new("toli"), new("rimti"),
            new("kilti"), new("likti"), new("mirti"), new("loti"),
            new("kiloti"), new("ly"), new("tirti"), new("irti"),
            new("kloti"), new("lyti"), new("tiras"), new("rasit"),
            new("rasti"), new("labas"), new("rytas"), new("balas"),
            new("tyras"), new("baslys"), new("tara"), new("ryti")
        });

        var anagrams = await _anagramSolver.GetAnagrams(input, 2);
        
        anagrams.ShouldNotBeNull();
        anagrams.Count.ShouldBe(expectedCount);
        foreach (var expectedAnagram in expectedAnagrams)
        {
            expectedAnagram.ShouldBeOneOf(anagrams.ToArray());
        }
        _wordRepository.Received().ReadWords();
    }
}