using BusinessLogic.Helpers;
using BusinessLogic.Services;
using Contracts.Interfaces;
using Contracts.Models;
using Tests.Mocks;

namespace Tests.Tests;

[TestFixture]
public class AnagramSolverTests
{
    private IAnagramSolver _anagramSolver;

    [SetUp]
    public void Setup()
    {
        var serviceResolver = new ServiceResolver(key => key switch
        {
            RepositoryType.File => new MockWordRepo(1),
            RepositoryType.Db => new MockWordRepo(1)
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

        var anagrams = await _anagramSolver.GetAnagrams(input, 2);
        
        anagrams.ShouldNotBeNull();
        anagrams.Count.ShouldBe(expectedCount);
    }
    
    [TestCase("alus", "sula")]
    [TestCase("toli  ", "loti")]
    [TestCase("geras kava", "keras vaga")]
    [TestCase("geras   kava", "keras vaga")]
    [TestCase("toli rimti kilti", "loti mirti likti")]
    public async Task GetAnagrams_IfInputWithOneAnagramGiven_ListOfAnagrams(string input, string output)
    {
        var expectedCount = 1;
        var expectedAnagram = new AnagramModel(output);

        var anagrams = await _anagramSolver.GetAnagrams(input, 2);
        
        anagrams.ShouldNotBeNull();
        anagrams.Count.ShouldBe(expectedCount);
        expectedAnagram.ShouldBeOneOf(anagrams.ToArray());
    }
    
    [TestCase("tiras", new []{"rasit", "rasti"})]
    [TestCase("tiras   ", new []{"rasit", "rasti"})]
    [TestCase("labas rytas", new []{"balas tyras", "baslys tara"})]
    [TestCase("toli ryti kilti", new []{"irti kloti lyti", "kiloti ly tirti"})]
    public async Task GetAnagrams_IfInputWithMultipleAnagramsGiven_ListOfAnagrams(string input, string[] output)
    {
        var expectedCount = 2;
        var expectedAnagrams = output.Select(x => new AnagramModel(x)).ToArray();

        var anagrams = await _anagramSolver.GetAnagrams(input, 2);
        
        anagrams.ShouldNotBeNull();
        anagrams.Count.ShouldBe(expectedCount);
        foreach (var expectedAnagram in expectedAnagrams)
        {
            expectedAnagram.ShouldBeOneOf(anagrams.ToArray());
        }
    }
}