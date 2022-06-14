using BusinessLogic.Services;
using Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tests.Mocks;
using WebApp.Controllers;
using WebApp.Helpers;
using WebApp.Models;

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
        
        _homeController = new HomeController(anagramSolver, wordRepo, wordService, wordSettings);
    }

    [Test]
    public void Index_IfNoWordGiven_ReturnsEmptyView()
    {
        var result = _homeController.Index(new AnagramViewModel()) as ViewResult;
        
        result.ViewName.ShouldBe("Index");
        result.Model.ShouldBeAssignableTo<AnagramViewModel>();
        result.Model.ShouldBeNull();
    }
    
    [Test]
    public void Index_WhenTooShortWordGiven_ReturnsEmptyView()
    {
        var model = new AnagramViewModel
        {
            SearchString = "ad"
        };

        var result = _homeController.Index(model) as ViewResult;
        model = result.Model as AnagramViewModel;
        
        result.ViewName.ShouldBe("Index");
        result.Model.ShouldBeAssignableTo<AnagramViewModel>();
        model.Anagrams.ShouldBeNull();
        model.ErrorMessage.ShouldBe("The given word was too short");
    }
    
    [Test]
    public void Index_IfWordWithoutAnagramsGiven_ReturnsWordAnagrams()
    {
        var model = new AnagramViewModel()
        {
            SearchString = "rega"
        };
        
        var result = _homeController.Index(model) as ViewResult;
        model = result.Model as AnagramViewModel;
        
        result.ViewName.ShouldBe("Index");
        result.Model.ShouldNotBeNull();
        result.Model.ShouldBeAssignableTo<AnagramViewModel>();
        model.Anagrams.Count.ShouldBe(0);
        model.ErrorMessage.ShouldBe("No anagrams found with given word");
    }
    
    [Test]
    public void Index_IfWordWithGiven_ReturnsWordAnagrams()
    {
        var model = new AnagramViewModel()
        {
            SearchString = "alus"
        };
        
        var result = _homeController.Index(model) as ViewResult;
        model = result.Model as AnagramViewModel;
        
        result.ViewName.ShouldBe("Index");
        result.Model.ShouldNotBeNull();
        result.Model.ShouldBeAssignableTo<AnagramViewModel>();
        model.Anagrams.Count.ShouldBe(1);
        model.ErrorMessage.ShouldBe("");
    }
    
    [Test]
    public void AllWordsList_IfNoWordWasGiven_ReturnsAPaginatedListOfWords()
    {
        var result = _homeController.AllWordsList(1, null) as ViewResult;
        var model = result.Model as PaginatedList<Word>;
        
        result.ViewName.ShouldBe("AllWordsList");
        result.Model.ShouldNotBeNull();
        result.Model.ShouldBeAssignableTo<PaginatedList<Word>>();
        model.Count.ShouldBe(30);
    }
    
    [Test]
    public void AllWordsList_IfWordWasGiven_RedirectsToIndex()
    {
        var result = _homeController.AllWordsList(1, "alus") as RedirectToActionResult;

        result.ActionName.ShouldBe("Index");
    }
    
    [Test]
    public void AddWordToDictionary_IfNoWordWasGiven_ReturnsEmptyView()
    {
        var result = _homeController.AddWordToDictionary(new WordViewModel()) as ViewResult;
        
        result.ViewName.ShouldBe("AddWordToDictionary");
        result.Model.ShouldBeNull();
        result.Model.ShouldBeAssignableTo<WordViewModel>();
    }
    
    [Test]
    public void AddWordToDictionary_IfExistingWordWasGiven_ReturnsFailureMessage()
    {
        var model = new WordViewModel()
        {
            Word = "alus"
        };
        var result = _homeController.AddWordToDictionary(model) as ViewResult;
        model = result.Model as WordViewModel;
        
        result.ViewName.ShouldBe("AddWordToDictionary");
        result.Model.ShouldNotBeNull();
        result.Model.ShouldBeAssignableTo<WordViewModel>();
        model.Message.ShouldBe("Word is already in the dictionary");
    }
    
    [Test]
    public void AddWordToDictionary_IfNotExistingWordWasGiven_ReturnsSuccessMessage()
    {
        var model = new WordViewModel()
        {
            Word = "poryt"
        };
        var result = _homeController.AddWordToDictionary(model) as ViewResult;
        model = result.Model as WordViewModel;
        
        result.ViewName.ShouldBe("AddWordToDictionary");
        result.Model.ShouldNotBeNull();
        result.Model.ShouldBeAssignableTo<WordViewModel>();
        model.Message.ShouldBe("Word added successfully");
    }
    
    [Test]
    public void AddWordToDictionary_IfMultipleWordsWereGiven_ReturnsFailureMessage()
    {
        var model = new WordViewModel()
        {
            Word = "poryt eisime"
        };
        var result = _homeController.AddWordToDictionary(model) as ViewResult;
        model = result.Model as WordViewModel;
        
        result.ViewName.ShouldBe("AddWordToDictionary");
        result.Model.ShouldNotBeNull();
        result.Model.ShouldBeAssignableTo<WordViewModel>();
        model.Message.ShouldBe("You are only allowed to enter one word at the time");
    }
}