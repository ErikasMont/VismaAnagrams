using System.Net;
using BusinessLogic.Helpers;
using BusinessLogic.Services;
using Contracts.Models;
using Microsoft.AspNetCore.Http;
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
    private HomeController _homeControllerWithSearches;
    private HomeController _homeControllerWithoutSearches;
    private HttpContextAccessor _accessor;
    
    [SetUp]
    public void Setup()
    {
        var serviceResolverNotEmpty = new ServiceResolver(key => key switch
        {
            RepositoryType.File => new MockWordRepo(1),
            RepositoryType.Db => new MockWordRepo(1)
        });
        
        var serviceResolverEmpty = new ServiceResolver(key => key switch
        {
            RepositoryType.File => new MockWordRepo(2),
            RepositoryType.Db => new MockWordRepo(2)
        });
        
        var wordServiceNotEmpty = new WordService(serviceResolverNotEmpty);
        var wordServiceEmpty = new WordService(serviceResolverEmpty);
        var anagramSolver = new AnagramSolver(wordServiceNotEmpty);
        var userServiceWithSearches = new UserService(new MockUserRepo(1));
        var userServiceWithoutSearches = new UserService(new MockUserRepo(2));
        var wordSettings = Options.Create<WordSettings>(new WordSettings());
        wordSettings.Value.AnagramCount = 3;
        wordSettings.Value.MinInputLength = 4;
        
        _homeControllerWithSearches = new HomeController(anagramSolver, wordServiceNotEmpty, userServiceWithSearches, wordSettings);
        _homeControllerWithSearches.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _homeControllerWithSearches.ControllerContext.HttpContext.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
        
        _homeControllerWithoutSearches = new HomeController(anagramSolver, wordServiceEmpty, userServiceWithoutSearches, wordSettings);
        _homeControllerWithoutSearches.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _homeControllerWithoutSearches.ControllerContext.HttpContext.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
    }

    [Test]
    public async Task Index_IfNoWordGiven_ReturnsEmptyView()
    {
        var result = await _homeControllerWithSearches.Index(new AnagramViewModel()) as ViewResult;
        
        result.ViewName.ShouldBe("Index");
        result.Model.ShouldBeAssignableTo<AnagramViewModel>();
        result.Model.ShouldBeNull();
    }
    
    [Test]
    public async Task Index_WhenTooShortWordGiven_ReturnsEmptyView()
    {
        var model = new AnagramViewModel
        {
            SearchString = "ad"
        };

        var result = await _homeControllerWithSearches.Index(model) as ViewResult;
        model = result.Model as AnagramViewModel;
        
        result.ViewName.ShouldBe("Index");
        result.Model.ShouldBeAssignableTo<AnagramViewModel>();
        model.Anagrams.ShouldBeNull();
        model.ErrorMessage.ShouldBe("The given word was too short");
    }
    
    [Test]
    public async Task Index_IfWordWithoutAnagramsGiven_ReturnsWordAnagrams()
    {
        var model = new AnagramViewModel()
        {
            SearchString = "visma"
        };
        
        var result = await _homeControllerWithSearches.Index(model) as ViewResult;
        model = result.Model as AnagramViewModel;
        
        result.ViewName.ShouldBe("Index");
        result.Model.ShouldNotBeNull();
        result.Model.ShouldBeAssignableTo<AnagramViewModel>();
        model.Anagrams.Count.ShouldBe(0);
        model.ErrorMessage.ShouldBe("No anagrams found with given word");
    }
    
    [Test]
    public async Task Index_IfWordWithGiven_ReturnsWordAnagrams()
    {
        var model = new AnagramViewModel()
        {
            SearchString = "alus"
        };
        
        var result = await _homeControllerWithSearches.Index(model) as ViewResult;
        model = result.Model as AnagramViewModel;
        
        result.ViewName.ShouldBe("Index");
        result.Model.ShouldNotBeNull();
        result.Model.ShouldBeAssignableTo<AnagramViewModel>();
        model.Anagrams.Count.ShouldBe(1);
        model.ErrorMessage.ShouldBe("");
    }
    
    [Test]
    public async Task Index_IfUserHasNoSearchesLeft_ReturnsErrorMessage()
    {
        var model = new AnagramViewModel()
        {
            SearchString = "alus"
        };
        
        var result = await _homeControllerWithoutSearches.Index(model) as ViewResult;
        model = result.Model as AnagramViewModel;
        
        result.ViewName.ShouldBe("Index");
        model.ErrorMessage.ShouldBe("You have reached the limit of your searches. " + 
                                    "To get more searches add new or correct existing word to the dictionary");
    }
    
    [Test]
    public async Task AllWordsList_IfNoWordWasGiven_ReturnsAPaginatedListOfWords()
    {
        var result = await _homeControllerWithSearches.AllWordsList(1, null) as ViewResult;
        var model = result.Model as PaginatedList<Word>;
        
        result.ViewName.ShouldBe("AllWordsList");
        result.Model.ShouldNotBeNull();
        result.Model.ShouldBeAssignableTo<PaginatedList<Word>>();
        model.Count.ShouldBe(30);
    }
    
    [Test]
    public async Task AllWordsList_IfWordWasGiven_RedirectsToIndex()
    {
        var result = await _homeControllerWithSearches.AllWordsList(1, "alus") as RedirectToActionResult;

        result.ActionName.ShouldBe("Index");
    }
    
    [Test]
    public async Task AddWordToDictionary_IfNoWordWasGiven_ReturnsEmptyView()
    {
        var result = await _homeControllerWithSearches.AddWordToDictionary(new WordViewModel()) as ViewResult;
        
        result.ViewName.ShouldBe("AddWordToDictionary");
        result.Model.ShouldBeNull();
        result.Model.ShouldBeAssignableTo<WordViewModel>();
    }
    
    [Test]
    public async Task AddWordToDictionary_IfExistingWordWasGiven_ReturnsFailureMessage()
    {
        var model = new WordViewModel()
        {
            Word = "alus"
        };
        var result = await _homeControllerWithSearches.AddWordToDictionary(model) as ViewResult;
        model = result.Model as WordViewModel;
        
        result.ViewName.ShouldBe("AddWordToDictionary");
        result.Model.ShouldNotBeNull();
        result.Model.ShouldBeAssignableTo<WordViewModel>();
        model.Message.ShouldBe("Word is already in the dictionary");
    }
    
    [Test]
    public async Task AddWordToDictionary_IfNotExistingWordWasGiven_ReturnsSuccessMessage()
    {
        var model = new WordViewModel()
        {
            Word = "poryt"
        };
        var result = await _homeControllerWithSearches.AddWordToDictionary(model) as ViewResult;
        model = result.Model as WordViewModel;
        
        result.ViewName.ShouldBe("AddWordToDictionary");
        result.Model.ShouldNotBeNull();
        result.Model.ShouldBeAssignableTo<WordViewModel>();
        model.Message.ShouldBe("Word added successfully");
    }
    
    [Test]
    public async Task AddWordToDictionary_IfMultipleWordsWereGiven_ReturnsFailureMessage()
    {
        var model = new WordViewModel()
        {
            Word = "poryt eisime"
        };
        var result = await _homeControllerWithSearches.AddWordToDictionary(model) as ViewResult;
        model = result.Model as WordViewModel;
        
        result.ViewName.ShouldBe("AddWordToDictionary");
        result.Model.ShouldNotBeNull();
        result.Model.ShouldBeAssignableTo<WordViewModel>();
        model.Message.ShouldBe("You are only allowed to enter one word at the time");
    }

    [Test]
    public async Task RemoveWordFromDictionary_IfUserDoesNotHaveAnySearchesLeft_RedirectsToIndex()
    {
        var result = await _homeControllerWithoutSearches.RemoveWordFromDictionary("alus") as RedirectToActionResult;
        
        result.ActionName.ShouldBe("Index");
    }
    
    [Test]
    public async Task RemoveWordFromDictionary_IfUserHasSearchesLeft_RedirectsToAllWordsList()
    {
        var result = await _homeControllerWithSearches.RemoveWordFromDictionary("alus") as RedirectToActionResult;
        
        result.ActionName.ShouldBe("AllWordsList");
    }
    
    [Test]
    public async Task EditWord_IfWordAlreadyExists_RedirectsToIndex()
    {
        var model = new EditWordViewModel() { ExistingWord = "alus", EditedWord = "alus" };
        
        var result = await _homeControllerWithSearches.EditWord(model) as RedirectToActionResult;
        
        result.ActionName.ShouldBe("Index");
    }
    
    [Test]
    public async Task EditWord_IfWordDoesNotAlreadyExist_RedirectsToAllWordsList()
    {
        var model = new EditWordViewModel() { ExistingWord = "alus", EditedWord = "aluss" };
        
        var result = await _homeControllerWithSearches.EditWord(model) as RedirectToActionResult;
        
        result.ActionName.ShouldBe("AllWordsList");
    }

    [Test]
    public async Task SearchWords_IfNoWordsFound_ReturnsErrorMessage()
    {
        var model = new SearchWordViewModel() { SearchString = "cccc" };

        var result = await _homeControllerWithoutSearches.SearchWords(model) as ViewResult;
        var resultModel = result.Model as SearchWordViewModel;
        
        result.ViewName.ShouldBe("SearchWords");
        resultModel.ErrorMessage.ShouldBe("No words found.");
    }
}