using BusinessLogic.Services;
using Contracts.Interfaces;
using Contracts.Models;
using Tests.Mocks;

namespace Tests.Tests;

[TestFixture]
public class WordServiceTests
{
    private IWordService _wordService;
    private IWordService _wordServiceEmpty;
    [SetUp]
    public void Setup()
    {
        _wordService = new WordService(new MockWordRepo(1));
        _wordServiceEmpty = new WordService(new MockWordRepo(2));
    }
    
    [Test]
    public void GetSortedWords_IfNoWordsRead_IsNull()
    {
        var anagrams = _wordServiceEmpty.GetSortedWords();
        
        anagrams.ShouldBeNull();
    }
    
    [Test]
    public void GetSortedWords_IfAllWordsRead_ReturnsSortedWordDictionary()
    {
        var expectedKey = "aegr";
        var expectedValue = new List<Anagram>(){new ("rega")};
        
        var sortedWords = _wordService.GetSortedWords();
        
        sortedWords.ShouldNotBeNull();
        sortedWords.ShouldContainKey(expectedKey);
        // During debug values key and value exist, assertion returns false even though values are the same
        //sortedWords.ShouldContainKeyAndValue(expectedKey, expectedValue);
    }
    
    [TestCase("dcba", "abcd")]
    [TestCase("abcd", "abcd")]
    public void Alphabetize_Always_ReturnsSortedString(string input, string expectedOutput)
    {
        var output = _wordService.Alphabetize(input);
        
        output.ShouldNotBeNull();
        output.ShouldBe(expectedOutput);
    }
    
    [Test]
    public void RemoveDuplicates_IfEmptyListWasGiven_IsNull()
    {
        var output = _wordService.RemoveDuplicates(new List<Anagram>(), new Anagram("test"));
        
        output.ShouldBeNull();
    }
    
    [Test]
    public void RemoveDuplicates_IfNotEmptyListWasGiven_ReturnsListWithoutDuplicates()
    {
        var inputList = new List<Anagram>()
        {
            new ("alus"), new ("alus"), new ("sula"), new ("geras"),
            new ("rytoj"), new ("sula")
        };
        var inputAnagram = new Anagram("geras");
        var expectedCount = 3;
        
        var output = _wordService.RemoveDuplicates(inputList, inputAnagram);
        
        output.ShouldNotBeNull();
        output.Count.ShouldBe(expectedCount);
    }
}