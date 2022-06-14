using BusinessLogic.Services;
using Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tests.Mocks;
using WebApp.Controllers;
using WebApp.Helpers;

namespace Tests.Tests;

[TestFixture]
public class HomeControllerTests
{
    private HomeController _homeController;
    
    [SetUp]
    public void Setup()
    {
        var wordRepo = new MockWordRepo(1);
        var wordService = new WordService(wordRepo);
        var anagramSolver = new AnagramSolver(wordService);
        var wordSettings = Options.Create<WordSettings>(new WordSettings());
        wordSettings.Value.AnagramCount = 3;
        wordSettings.Value.MinInputLength = 4;
        
        _homeController = new HomeController(anagramSolver, wordRepo, wordSettings);
    }

    [Test]
    public void Index_IfNoWordGiven_ReturnsEmptyView()
    {
        var result = _homeController.Index(null) as ViewResult;
        
        result.ViewName.ShouldBe("Index");
        result.Model.ShouldBeNull();
    }
    
    [Test]
    public void Index_IfWordGiven_ReturnsWordAnagrams()
    {
        var result = _homeController.Index("alus") as ViewResult;
        var model = result.Model as List<Anagram>;
        
        result.ViewName.ShouldBe("Index");
        result.Model.ShouldNotBeNull();
        result.Model.ShouldBeAssignableTo<List<Anagram>>();
        model.Count.ShouldBe(1);
    }
    
    [Test]
    public void AllWordsList_Always_ReturnsAPaginatedListOfWords()
    {
        var result = _homeController.AllWordsList(1) as ViewResult;
        var model = result.Model as PaginatedList<Word>;
        
        result.ViewName.ShouldBe("AllWordsList");
        result.Model.ShouldNotBeNull();
        result.Model.ShouldBeAssignableTo<PaginatedList<Word>>();
        model.Count.ShouldBe(30);
    }
}