using BusinessLogic.DataAccess;
using BusinessLogic.Services;
using Contracts.Interfaces;
using Contracts.Models;

namespace Tests;

[TestFixture]
public class AnagramSolverTests
{
    private IAnagramSolver _anagramSolver;

    [OneTimeSetUp]
    public void Setup()
    {
        _anagramSolver = new AnagramSolver(new WordDataAccess());
    }

    [TestCase("rega")]
    [TestCase("          ")]
    [TestCase("")]
    [TestCase("visma kava")]
    public void GetAanagrams_IfOneWordWithoutAnagramsGiven_EmptyListOfAnagrams(string input)
    {
        var expectedCount = 0;

        var anagrams = _anagramSolver.GetAnagrams(input, 2);
        
        Assert.That(anagrams, Is.TypeOf(typeof(List<Anagram>)));
        Assert.That(anagrams, Has.Count.EqualTo(expectedCount));
    }
    
    [TestCase("alus", "sula")]
    [TestCase("toli  ", "loti")]
    [TestCase("geras kava", "keras vaga")]
    [TestCase("geras   kava", "keras vaga")]
    [TestCase("toli rimti kilti", "likti loti mirti")]
    public void GetAanagrams_IfOneWordWithOneAnagramGiven_ListOfAnagrams(string input, string output)
    {
        var expectedCount = 1;
        var expectedAnagram = new Anagram(output);

        var anagrams = _anagramSolver.GetAnagrams(input, 2);
        
        Assert.That(anagrams, Is.TypeOf(typeof(List<Anagram>)));
        Assert.That(anagrams, Has.Count.EqualTo(expectedCount));
        Assert.That(anagrams, Contains.Item(expectedAnagram));
    }
    
    [TestCase("tiras", new []{"rasit", "rasti"})]
    [TestCase("tiras   ", new []{"rasit", "rasti"})]
    [TestCase("labas rytas", new []{"balas tyras", "baslys tara"})]
    [TestCase("toli ryti kilti", new []{"irti kloti lyti", "kiloti ly tirti"})]
    public void GetAanagrams_IfOneWordWithMultipleAnagramsGiven_ListOfAnagrams(string input, string[] output)
    {
        var expectedCount = 2;
        var expectedAnagrams = output.Select(x => new Anagram(x)).ToList();

        var anagrams = _anagramSolver.GetAnagrams(input, 2);
        
        Assert.That(anagrams, Is.TypeOf(typeof(List<Anagram>)));
        Assert.That(anagrams, Has.Count.EqualTo(expectedCount));
        foreach (var anagram in expectedAnagrams)
        {
            Assert.That(anagrams, Contains.Item(anagram));
        }
    }
}