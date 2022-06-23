using System.Text.Json;
using BusinessLogic.Helpers;
using BusinessLogic.Services;
using Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tests.Mocks;
using WebApp.Controllers;
using WebApp.Helpers;

namespace Tests.Tests;

[TestFixture]
public class AnagramControllerTests
{
    private AnagramController _anagramController;
    
    [SetUp]
    public void Setup()
    {
        var serviceResolver = new ServiceResolver(key => key switch
        {
            RepositoryType.File => new MockWordRepo(1),
            RepositoryType.Db => new MockWordRepo(1)
        });
        var wordService = new WordService(serviceResolver);
        var anagramSolver = new AnagramSolver(wordService);
        var wordSettings = Options.Create<WordSettings>(new WordSettings());
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

        var result = await _anagramController.GetAnagrams(input) as NotFoundObjectResult;

        result.StatusCode.ShouldBe(404);
        result.Value.ShouldBe(expectedValue);
    }
    
    [Test]
    public async Task GetAnagrams_WhenAnagramsFoundWithGivenInput_ReturnsOkStatusCode()
    {
        var input = "alus";
        var expectedValue = JsonSerializer.Serialize(new List<Anagram>() { new ("sula") });

        var result = await _anagramController.GetAnagrams(input) as OkObjectResult;

        result.StatusCode.ShouldBe(200);
        result.Value.ShouldBe(expectedValue);
    }
}