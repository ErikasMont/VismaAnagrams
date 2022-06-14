using BusinessLogic.Services;
using Contracts.Interfaces;
using Contracts.Models;
using Tests.Mocks;

namespace Tests.Tests;

[TestFixture]
public class WordServiceTests
{
    private IWordRepository _wordRepository;
    private IWordService _wordService;
    private IWordService _wordServiceEmpty;
    [SetUp]
    public void Setup()
    {
        _wordRepository = new MockWordRepo(1);
        _wordService = new WordService(_wordRepository);
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
    }
    
    [TestCase("dcba", "abcd")]
    [TestCase("abcd", "abcd")]
    public void Alphabetize_Always_ReturnsSortedString(string input, string expectedOutput)
    {
        var output = _wordService.Alphabetize(input);
        
        output.ShouldNotBeNull();
        output.ShouldBe(expectedOutput);
    }

    [TestCase("poryt eisim", new []{"poryt", "eisim"})]
    [TestCase("toli   tenais   ", new []{"toli", "tenais"})]
    public void ValidateInputWords_Always_ReturnsACorrectArrayOfWords(string input, string[] expectedOutput)
    {
        var expectedCount = 2;
        
        var output = _wordService.ValidateInputWords(input);

        output.ShouldNotBeNull();
        output.Length.ShouldBe(expectedCount);
        foreach (var word in output)
        {
            word.ShouldBeOneOf(expectedOutput);
        }
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

    [Test]
    public void AddWordToFile_WhenExistingWordGiven_ReturnsFalse()
    {
        var word = "alus";

        var result = _wordService.AddWordToFile(word);
        
        result.ShouldBeFalse();
    }
    
    [Test]
    public void AddWordToFile_WhenNotExistingWordGiven_ReturnsTrue()
    {
        var word = "testas";

        var result = _wordService.AddWordToFile(word);
        
        result.ShouldBeTrue();
    }
}