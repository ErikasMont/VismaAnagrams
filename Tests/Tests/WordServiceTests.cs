using BusinessLogic.Helpers;
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
        var serviceResolver = new ServiceResolver(key => 
            key switch
            {
                RepositoryType.File => new MockWordRepo(1),
                RepositoryType.Db => new MockWordRepo(1)
            });
        
        var serviceResolverEmpty = new ServiceResolver(key => 
            key switch
            {
                RepositoryType.File => new MockWordRepo(2),
                RepositoryType.Db => new MockWordRepo(2)
            });
        
        _wordService = new WordService(serviceResolver);
        _wordServiceEmpty = new WordService(serviceResolverEmpty);
    }
    
    [Test]
    public async Task GetSortedWords_IfNoWordsRead_EmptyDictionaryReturned()
    {
        var anagrams = await _wordServiceEmpty.GetSortedWords();
        
        anagrams.ShouldBeEmpty();
    }
    
    [Test]
    public async Task GetSortedWords_IfAllWordsRead_ReturnsSortedWordDictionary()
    {
        var expectedKey = "aegr";

        var sortedWords = await _wordService.GetSortedWords();
        
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
    public void RemoveDuplicates_IfEmptyListWasGiven_EmptyListReturned()
    {
        var output = _wordService.RemoveDuplicates(new List<Anagram>(), new Anagram("test"));
        
        output.ShouldBeEmpty();
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
    public async Task AddWordToFile_WhenExistingWordGiven_ReturnsFalse()
    {
        var word = "alus";

        var result = await _wordService.AddWordToFile(word);
        
        result.ShouldBeFalse();
    }
    
    [Test]
    public async Task AddWordToFile_WhenNotExistingWordGiven_ReturnsTrue()
    {
        var word = "testas";

        var result = await _wordService.AddWordToFile(word);
        
        result.ShouldBeTrue();
    }

    [Test]
    public async Task GetWordsList_Always_ReturnsAListOfWords()
    {
        var expectedCount = 30;
        
        var words = await _wordService.GetWordsList();
        
        words.Count.ShouldBe(expectedCount);
    }

    [Test]
    public async Task GetWordFromCache_IfNoWordFound_ReturnsNull()
    {
        var result = await _wordService.GetWordFromCache(new Word("toli"));
        
        result.ShouldBeNull();
    }

    [Test]
    public async Task GetWordFromCache_IfWordFound_ReturnsCachedWordModel()
    {
        var expectedAnagrams = "balas";

        var result = await _wordService.GetWordFromCache(new Word("labas"));

        result.ShouldNotBeNull();
        result.Anagrams.ShouldBe(expectedAnagrams);
    }

    [Test]
    public async Task SearchWords_Always_ReturnsAListOfWordsByGivenFilter()
    {
        var expectedWord = new Word("kava");

        var result = await _wordService.SearchWords("ava");
        result.ShouldNotBeEmpty();
        result.ShouldContain(expectedWord);
    }
}