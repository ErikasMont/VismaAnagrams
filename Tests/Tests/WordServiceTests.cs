using BusinessLogic.Helpers;
using BusinessLogic.Services;
using Contracts.Interfaces;
using Contracts.Models;
namespace Tests.Tests;

[TestFixture]
public class WordServiceTests
{
    private IWordService _wordService;
    private IWordRepository _wordRepository;
    [SetUp]
    public void Setup()
    {
        _wordRepository = Substitute.For<IWordRepository>();
        var serviceResolver = new ServiceResolver(key => 
            key switch
            {
                RepositoryType.File => _wordRepository,
                RepositoryType.Db => _wordRepository
            });

        _wordService = new WordService(serviceResolver);
    }
    
    [Test]
    public async Task GetSortedWords_IfNoWordsRead_EmptyDictionaryReturned()
    {
        _wordRepository.ReadWords().Returns(new List<Word>());
        
        var anagrams = await _wordService.GetSortedWords();
        
        anagrams.ShouldBeEmpty();
        _wordRepository.Received().ReadWords();
    }
    
    [Test]
    public async Task GetSortedWords_IfAllWordsRead_ReturnsSortedWordDictionary()
    {
        var expectedKey = "aegr";
        _wordRepository.ReadWords().Returns(new List<Word>()
        {
            new ("rega"), new ("gera"), new ("alus"), new ("sula")
        });

        var sortedWords = await _wordService.GetSortedWords();
        
        sortedWords.ShouldNotBeNull();
        sortedWords.ShouldContainKey(expectedKey);
        _wordRepository.Received().ReadWords();
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
    public async Task AddWord_WhenExistingWordGiven_ReturnsFalse()
    {
        var word = "alus";
        _wordRepository.ReadWords().Returns(new List<Word>()
        {
            new ("rega"), new ("gera"), new ("alus"), new ("sula")
        });

        var result = await _wordService.AddWord(word);
        
        result.ShouldBeFalse();
        _wordRepository.Received().ReadWords();
    }
    
    [Test]
    public async Task AddWord_WhenNotExistingWordGiven_ReturnsTrue()
    {
        var word = "testas";
        _wordRepository.ReadWords().Returns(new List<Word>()
        {
            new ("rega"), new ("gera"), new ("alus"), new ("sula")
        });

        var result = await _wordService.AddWord(word);
        
        result.ShouldBeTrue();
        _wordRepository.Received().ReadWords();
        _wordRepository.Received().WriteWord(new Word(word));
    }

    [Test]
    public async Task GetWordsList_Always_ReturnsAListOfWords()
    {
        var expectedCount = 30;
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
        
        var words = await _wordService.GetWordsList();
        
        words.Count.ShouldBe(expectedCount);
        _wordRepository.Received().ReadWords();
    }

    [Test]
    public async Task GetWordFromCache_IfNoWordFound_ReturnsNull()
    {
        _wordRepository.ReadWordsFromCache().Returns(new List<CachedWord>());
        
        var result = await _wordService.GetWordFromCache(new Word("toli"));
        
        result.ShouldBeNull();
        _wordRepository.Received().ReadWordsFromCache();
    }

    [Test]
    public async Task GetWordFromCache_IfWordFound_ReturnsCachedWordModel()
    {
        var expectedAnagrams = "balas";
        _wordRepository.ReadWordsFromCache().Returns(new List<CachedWord>()
        {
            new ("labas", "balas"), new ("alus", "sula")
        });

        var result = await _wordService.GetWordFromCache(new Word("labas"));

        result.ShouldNotBeNull();
        result.Anagrams.ShouldBe(expectedAnagrams);
        _wordRepository.Received().ReadWordsFromCache();
    }

    [Test]
    public async Task SearchWords_Always_ReturnsAListOfWordsByGivenFilter()
    {
        var expectedWord = new Word("kava");
        _wordRepository.SearchWordsByFilter("ava").Returns(new List<Word>()
        {
            expectedWord
        });

        var result = await _wordService.SearchWords("ava");
        
        result.ShouldNotBeEmpty();
        result.ShouldContain(expectedWord);
        _wordRepository.Received().SearchWordsByFilter("ava");
    }

    [Test]
    public async Task EditWord_IfWordAlreadyExists_ReturnsFalse()
    {
        var existingWord = "alus";
        var editedWord = "alus";
        _wordRepository.ReadWords().Returns(new List<Word>()
        {
            new ("alus"), new ("sula")
        });

        var result = await _wordService.EditWord(existingWord, editedWord);
        
        result.ShouldBeFalse();
        _wordRepository.Received().ReadWords();
    }
    
    [Test]
    public async Task EditWord_IfWordDoesNotAlreadyExist_ReturnsFalse()
    {
        var existingWord = "alus";
        var editedWord = "aluss";
        _wordRepository.ReadWords().Returns(new List<Word>()
        {
            new ("alus"), new ("sula")
        });

        var result = await _wordService.EditWord(existingWord, editedWord);
        
        result.ShouldBeTrue();
        _wordRepository.Received().ReadWords();
    }
}