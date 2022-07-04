using System.Text.Json;
using BusinessLogic.Helpers;
using BusinessLogic.Services;
using Contracts.Interfaces;
using Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp.Controllers;
using WebApp.Helpers;

namespace Tests.Tests;

[TestFixture]
public class AnagramControllerTests
{
    private AnagramController _anagramController;
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
        var wordService = new WordService(serviceResolver);
        var anagramSolver = new AnagramSolver(wordService);
        var wordSettings = Options.Create(new WordSettings());
        wordSettings.Value.AnagramCount = 3;
        wordSettings.Value.MinInputLength = 4;
        
        _anagramController = new AnagramController(anagramSolver, wordSettings);
    }

    [Test]
    public async Task GetAnagrams_WhenInputIsTooShort_ReturnsBadRequestStatusCode()
    {
        var input = "alu";
        var expectedValue = "Input was too short.";

        var result = await _anagramController.GetAnagrams(input) as BadRequestObjectResult;

        result.StatusCode.ShouldBe(400);
        result.Value.ShouldBe(expectedValue);
    }
    
    [Test]
    public async Task GetAnagrams_WhenNoAnagramsFoundWithGivenInput_ReturnsNotFoundStatusCode()
    {
        var input = "ryti";
        var expectedValue = "No anagrams found with given input.";
        _wordRepository.ReadWords().Returns(new List<Word>()
        {
            new ("alus"), new ("sula")
        });

        var result = await _anagramController.GetAnagrams(input) as NotFoundObjectResult;

        result.StatusCode.ShouldBe(404);
        result.Value.ShouldBe(expectedValue);
        _wordRepository.Received().ReadWords();
    }
    
    [Test]
    public async Task GetAnagrams_WhenAnagramsFoundWithGivenInput_ReturnsOkStatusCode()
    {
        var input = "alus";
        var expectedValue = JsonSerializer.Serialize(new List<Anagram>() { new ("sula") });
        _wordRepository.ReadWords().Returns(new List<Word>()
        {
            new ("alus"), new ("sula")
        });

        var result = await _anagramController.GetAnagrams(input) as OkObjectResult;

        result.StatusCode.ShouldBe(200);
        result.Value.ShouldBe(expectedValue);
        _wordRepository.Received().ReadWords();
    }
}