using Contracts.Interfaces;
using Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp.Helpers;
using WebApp.Models;

namespace WebApp.Controllers;
public class HomeController : Controller
{
    private readonly IAnagramSolver _anagramSolver;
    private readonly IWordService _wordService;
    private readonly IUserService _userService;
    private readonly WordSettings _wordSettings;

    public HomeController(IAnagramSolver anagramSolver, IWordService wordService, IUserService userService, 
        IOptions<WordSettings> wordSettings)
    {
        _anagramSolver = anagramSolver;
        _wordSettings = wordSettings.Value;
        _wordService = wordService;
        _userService = userService;
    }
    
    public async Task<IActionResult> Index(AnagramViewModel? model)
    {
        var userIp = HttpContext.Connection.RemoteIpAddress == null
            ? "" : HttpContext.Connection.RemoteIpAddress?.ToString();
        
        if (!await _userService.UserExists(userIp))
        {
            await _userService.AddUser(userIp);
        }
        
        if (model.SearchString == null)
        {
            return View("Index");
        }
        
        if (model.SearchString.Length < _wordSettings.MinInputLength)
        {
            await _wordService.AddSearch(userIp, model.SearchString,
                new List<Anagram>());
            model.ErrorMessage = "The given word was too short";
            return View("Index", model);
        }

        var user = await _userService.GetUser(userIp);
        if (user.SearchesLeft == 0)
        {
            model.ErrorMessage =
                "You have reached the limit of your searches. " +
                "To get more searches add new or correct existing word to the dictionary";
            
            return View("Index", model);
        }
        
        var cachedWord = await _wordService.GetWordFromCache(new Word(model.SearchString));

        List<Anagram>? anagrams;
        if (cachedWord == null)
        {
            anagrams = await _anagramSolver.GetAnagrams(model.SearchString, _wordSettings.AnagramCount);
            await _wordService.WriteWordToCache(new Word(model.SearchString), anagrams);
            model.Anagrams = anagrams;
            model.ErrorMessage = "";
            if (!model.Anagrams.Any())
            {
                model.ErrorMessage = "No anagrams found with given word";
            }
            await _wordService.AddSearch(userIp, model.SearchString, anagrams);
            await _userService.UpdateSearchCount("reduce", userIp);
        
            return View("Index", model);
        }

        var cachedAnagrams = _wordService.ValidateInputWords(cachedWord.Anagrams).ToList();
        anagrams = cachedAnagrams.Select(x => new Anagram(x)).ToList();
        model.Anagrams = anagrams;
        model.ErrorMessage = "";
        if (!model.Anagrams.Any())
        {
            model.ErrorMessage = "No anagrams found with given word";
        }
        await _wordService.AddSearch(userIp, model.SearchString, anagrams);
        await _userService.UpdateSearchCount("reduce", userIp);
        
        return View("Index", model);
    }

    public async Task<IActionResult> AllWordsList(int? pageNumber, string? searchString)
    {
        if (searchString != null)
        {
            var model = new AnagramViewModel()
            {
                SearchString = searchString
            };
            return RedirectToAction("Index", model);
        }
        var words = await _wordService.GetWordsList();

        return View("AllWordsList", PaginatedList<Word>.Create(words, pageNumber ?? 1, 100));
    }

    public async Task<IActionResult> RemoveWordFromDictionary(string word)
    {
        var userIp = HttpContext.Connection.RemoteIpAddress == null
            ? "" : HttpContext.Connection.RemoteIpAddress?.ToString();
        var user = await _userService.GetUser(userIp);
        if (user.SearchesLeft == 0)
        {
            return RedirectToAction("Index");
        }
        await _wordService.RemoveWord(word);
        await _userService.UpdateSearchCount("reduce", userIp);

        return RedirectToAction("AllWordsList");
    }

    public IActionResult EditWordForm(string word)
    {
        var model = new EditWordViewModel() { ExistingWord = word};
        return View("EditWordForm", model);
    }

    public async Task<IActionResult> EditWord(EditWordViewModel model)
    {
        var successful = await _wordService.EditWord(model.ExistingWord, model.EditedWord);
        if (!successful)
        {
            return RedirectToAction("Index");
        }
        
        var userIp = HttpContext.Connection.RemoteIpAddress == null
            ? "" : HttpContext.Connection.RemoteIpAddress?.ToString();
        await _userService.UpdateSearchCount("increase", userIp);
        return RedirectToAction("AllWordsList");
    }

    public async Task<IActionResult> AddWordToDictionary(WordViewModel? model)
    {
        if (model.Word == null)
        {
            return View("AddWordToDictionary");
        }

        var inputWords = _wordService.ValidateInputWords(model.Word);
        if (inputWords.Length > 1)
        {
            model.Message = "You are only allowed to enter one word at the time";
            return View("AddWordToDictionary", model);
        }

        var successful = await _wordService.AddWord(inputWords[0]);
        if (successful)
        {
            var userIp = HttpContext.Connection.RemoteIpAddress == null
                ? "" : HttpContext.Connection.RemoteIpAddress?.ToString();
            await _userService.UpdateSearchCount("increase", userIp);
            model.Message = "Word added successfully";
            return View("AddWordToDictionary", model);
        }

        model.Message = "Word is already in the dictionary";
        return View("AddWordToDictionary", model);
    }

    public IActionResult BrowserStorageValuesList()
    {
        return View();
    }

    public async Task<IActionResult> SearchWords(SearchWordViewModel? model)
    {
        if (model.SearchString == null)
        {
            return View("SearchWords");
        }

        var searchWords = await _wordService.SearchWords(model.SearchString);
        model.Words = searchWords.ToList();
        model.ErrorMessage = "";
        if (!model.Words.Any())
        {
            model.ErrorMessage = "No words found.";
        }

        return View("SearchWords", model);
    }
}