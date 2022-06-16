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
    private readonly IWordRepository _wordRepository;
    private readonly IWordService _wordService;
    private readonly WordSettings _wordSettings;

    public HomeController(IAnagramSolver anagramSolver, IWordRepository wordRepository, 
        IWordService wordService, IOptions<WordSettings> wordSettings)
    {
        _anagramSolver = anagramSolver;
        _wordRepository = wordRepository;
        _wordSettings = wordSettings.Value;
        _wordService = wordService;
    }
    
    public IActionResult Index(AnagramViewModel? model)
    {
        if (model.SearchString == null)
        {
            return View("Index");
        }

        if (model.SearchString.Length < _wordSettings.MinInputLength)
        {
            model.ErrorMessage = "The given word was too short";
            return View("Index", model);
        }
        
        model.Anagrams = _anagramSolver.GetAnagrams(model.SearchString, _wordSettings.AnagramCount);
        model.ErrorMessage = "";
        if (!model.Anagrams.Any())
        {
            model.ErrorMessage = "No anagrams found with given word";
        }
        
        return View("Index", model);
    }

    public IActionResult AllWordsList(int? pageNumber, string? searchString)
    {
        if (searchString != null)
        {
            AnagramViewModel model = new AnagramViewModel()
            {
                SearchString = searchString
            };
            return RedirectToAction("Index", model);
        }
        var words = _wordRepository.ReadWords().Distinct().ToList();

        return View("AllWordsList", PaginatedList<Word>.Create(words, pageNumber ?? 1, 100));
    }

    public IActionResult AddWordToDictionary(WordViewModel? model)
    {
        if (model.Word == null)
        {
            return View("AddWordToDictionary");
        }

        var inputWords = _wordService.ValidateInputWords(model.Word);
        if (inputWords.Length is > 1)
        {
            model.Message = "You are only allowed to enter one word at the time";
            return View("AddWordToDictionary", model);
        }

        var successful = _wordService.AddWordToFile(inputWords[0]);
        if (successful)
        {
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
}