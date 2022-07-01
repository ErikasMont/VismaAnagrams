using System.Net;
using BusinessLogic.Helpers;
using BusinessLogic.Services;
using Contracts.Interfaces;
using Contracts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp.Controllers;
using WebApp.Helpers;
using WebApp.Models;

namespace Tests.Tests;

[TestFixture]
public class HomeControllerTests
{
    private HomeController _homeController;
    private HttpContextAccessor _accessor;
    private IWordRepository _wordRepository;
    private IUserRepository _userRepository;
    
    [SetUp]
    public void Setup()
    {
        _wordRepository = Substitute.For<IWordRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        
        var serviceResolver = new ServiceResolver(key => key switch
        {
            RepositoryType.File => _wordRepository,
            RepositoryType.Db => _wordRepository
        });
        
        
        var wordService = new WordService(serviceResolver);
        var anagramSolver = new AnagramSolver(wordService);
        var userService = new UserService(_userRepository);
        var wordSettings = Options.Create(new WordSettings());
        wordSettings.Value.AnagramCount = 3;
        wordSettings.Value.MinInputLength = 4;
        
        _homeController = new HomeController(anagramSolver, wordService, userService, wordSettings);
        _homeController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _homeController.ControllerContext.HttpContext.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
    }

    [Test]
    public async Task Index_IfNoWordGiven_ReturnsEmptyView()
    {
        var result = await _homeController.Index(new AnagramViewModel()) as ViewResult;
        
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

        var result = await _homeController.Index(model) as ViewResult;
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
        _wordRepository.ReadWords().Returns(new List<Word>()
        {
            new ("alus"), new ("sula")
        });
        _userRepository.ReadUser("127.0.0.1").Returns(new User("127.0.0.1", 5));
        
        var result = await _homeController.Index(model) as ViewResult;
        model = result.Model as AnagramViewModel;
        
        result.ViewName.ShouldBe("Index");
        result.Model.ShouldNotBeNull();
        result.Model.ShouldBeAssignableTo<AnagramViewModel>();
        model.Anagrams.Count.ShouldBe(0);
        model.ErrorMessage.ShouldBe("No anagrams found with given word");
        _wordRepository.Received().ReadWords();
        _userRepository.Received().ReadUser("127.0.0.1");
    }
    
    [Test]
    public async Task Index_IfWordWithGiven_ReturnsWordAnagrams()
    {
        var model = new AnagramViewModel()
        {
            SearchString = "alus"
        };
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
        _userRepository.ReadUser("127.0.0.1").Returns(new User("127.0.0.1", 5));
        
        var result = await _homeController.Index(model) as ViewResult;
        model = result.Model as AnagramViewModel;
        
        result.ViewName.ShouldBe("Index");
        result.Model.ShouldNotBeNull();
        result.Model.ShouldBeAssignableTo<AnagramViewModel>();
        model.Anagrams.Count.ShouldBe(1);
        model.ErrorMessage.ShouldBe("");
        _wordRepository.Received().ReadWords();
        _userRepository.Received().ReadUser("127.0.0.1");
    }
    
    [Test]
    public async Task Index_IfUserHasNoSearchesLeft_ReturnsErrorMessage()
    {
        var model = new AnagramViewModel()
        {
            SearchString = "alus"
        };
        _userRepository.ReadUser("127.0.0.1").Returns(new User("127.0.0.1", 0));
        
        var result = await _homeController.Index(model) as ViewResult;
        model = result.Model as AnagramViewModel;
        
        result.ViewName.ShouldBe("Index");
        model.ErrorMessage.ShouldBe("You have reached the limit of your searches. " + 
                                    "To get more searches add new or correct existing word to the dictionary");
        _userRepository.Received().ReadUser("127.0.0.1");
    }
    
    [Test]
    public async Task AllWordsList_IfNoWordWasGiven_ReturnsAPaginatedListOfWords()
    {
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
        
        var result = await _homeController.AllWordsList(1, null) as ViewResult;
        var model = result.Model as PaginatedList<Word>;
        
        result.ViewName.ShouldBe("AllWordsList");
        result.Model.ShouldNotBeNull();
        result.Model.ShouldBeAssignableTo<PaginatedList<Word>>();
        model.Count.ShouldBe(30);
        _wordRepository.Received().ReadWords();
    }
    
    [Test]
    public async Task AllWordsList_IfWordWasGiven_RedirectsToIndex()
    {
        var result = await _homeController.AllWordsList(1, "alus") as RedirectToActionResult;

        result.ActionName.ShouldBe("Index");
    }
    
    [Test]
    public async Task AddWordToDictionary_IfNoWordWasGiven_ReturnsEmptyView()
    {
        var result = await _homeController.AddWordToDictionary(new WordViewModel()) as ViewResult;
        
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
        _wordRepository.ReadWords().Returns(new List<Word>()
        {
            new("alus")
        });
        
        var result = await _homeController.AddWordToDictionary(model) as ViewResult;
        model = result.Model as WordViewModel;
        
        result.ViewName.ShouldBe("AddWordToDictionary");
        result.Model.ShouldNotBeNull();
        result.Model.ShouldBeAssignableTo<WordViewModel>();
        model.Message.ShouldBe("Word is already in the dictionary");
        _wordRepository.Received().ReadWords();
    }
    
    [Test]
    public async Task AddWordToDictionary_IfNotExistingWordWasGiven_ReturnsSuccessMessage()
    {
        var model = new WordViewModel()
        {
            Word = "poryt"
        };
        _wordRepository.ReadWords().Returns(new List<Word>()
        {
            new("alus")
        });
        _userRepository.ReadUser("127.0.0.1").Returns(new User("127.0.0.1", 5));
        
        var result = await _homeController.AddWordToDictionary(model) as ViewResult;
        model = result.Model as WordViewModel;
        
        result.ViewName.ShouldBe("AddWordToDictionary");
        result.Model.ShouldNotBeNull();
        result.Model.ShouldBeAssignableTo<WordViewModel>();
        model.Message.ShouldBe("Word added successfully");
        _wordRepository.Received().ReadWords();
        _userRepository.Received().ReadUser("127.0.0.1");
    }
    
    [Test]
    public async Task AddWordToDictionary_IfMultipleWordsWereGiven_ReturnsFailureMessage()
    {
        var model = new WordViewModel()
        {
            Word = "poryt eisime"
        };
        var result = await _homeController.AddWordToDictionary(model) as ViewResult;
        model = result.Model as WordViewModel;
        
        result.ViewName.ShouldBe("AddWordToDictionary");
        result.Model.ShouldNotBeNull();
        result.Model.ShouldBeAssignableTo<WordViewModel>();
        model.Message.ShouldBe("You are only allowed to enter one word at the time");
    }

    [Test]
    public async Task RemoveWordFromDictionary_IfUserDoesNotHaveAnySearchesLeft_RedirectsToIndex()
    {
        _userRepository.ReadUser("127.0.0.1").Returns(new User("127.0.0.1", 0));
        
        var result = await _homeController.RemoveWordFromDictionary("alus") as RedirectToActionResult;
        
        result.ActionName.ShouldBe("Index");
        _userRepository.Received().ReadUser("127.0.0.1");
    }
    
    [Test]
    public async Task RemoveWordFromDictionary_IfUserHasSearchesLeft_RedirectsToAllWordsList()
    {
        _userRepository.ReadUser("127.0.0.1").Returns(new User("127.0.0.1", 5));
        
        var result = await _homeController.RemoveWordFromDictionary("alus") as RedirectToActionResult;
        
        result.ActionName.ShouldBe("AllWordsList");
        _userRepository.Received().ReadUser("127.0.0.1");
    }
    
    [Test]
    public async Task EditWord_IfWordAlreadyExists_RedirectsToIndex()
    {
        var model = new EditWordViewModel() { ExistingWord = "alus", EditedWord = "alus" };
        _wordRepository.ReadWords().Returns(new List<Word>()
        {
            new("alus"), new("sula")
        });
        
        var result = await _homeController.EditWord(model) as RedirectToActionResult;
        
        result.ActionName.ShouldBe("Index");
        _wordRepository.Received().ReadWords();
    }
    
    [Test]
    public async Task EditWord_IfWordDoesNotAlreadyExist_RedirectsToAllWordsList()
    {
        var model = new EditWordViewModel() { ExistingWord = "alus", EditedWord = "aluss" };
        _wordRepository.ReadWords().Returns(new List<Word>()
        {
            new("alus"), new("sula")
        });
        _userRepository.ReadUser("127.0.0.1").Returns(new User("127.0.0.1", 5));
        
        var result = await _homeController.EditWord(model) as RedirectToActionResult;
        
        result.ActionName.ShouldBe("AllWordsList");
        _wordRepository.Received().ReadWords();
        _userRepository.Received().ReadUser("127.0.0.1");
    }

    [Test]
    public async Task SearchWords_IfNoWordsFound_ReturnsErrorMessage()
    {
        var model = new SearchWordViewModel() { SearchString = "cccc" };
        _wordRepository.SearchWordsByFilter("cccc").Returns(new List<Word>());

        var result = await _homeController.SearchWords(model) as ViewResult;
        var resultModel = result.Model as SearchWordViewModel;
        
        result.ViewName.ShouldBe("SearchWords");
        resultModel.ErrorMessage.ShouldBe("No words found.");
        _wordRepository.Received().SearchWordsByFilter("cccc");
    }
}