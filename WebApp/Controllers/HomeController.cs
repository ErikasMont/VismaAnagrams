using Contracts.Interfaces;
using Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp.Helpers;

namespace WebApp.Controllers;
public class HomeController : Controller
{
    private readonly IAnagramSolver _anagramSolver;
    private readonly IWordRepository _wordRepository;
    private readonly WordSettings _wordSettings;

    public HomeController(IAnagramSolver anagramSolver, IWordRepository wordRepository, 
        IOptions<WordSettings> wordSettings)
    {
        _anagramSolver = anagramSolver;
        _wordRepository = wordRepository;
        _wordSettings = wordSettings.Value;
    }
    
    public IActionResult Index(string? word)
    {
        if (word == null)
        {
            return View("Index");
        }
        var anagrams = _anagramSolver.GetAnagrams(word, _wordSettings.AnagramCount);
        return View("Index", anagrams);
    }

    public IActionResult AllWordsList(int? pageNumber)
    {
        var words = _wordRepository.ReadWords().Distinct().ToList();

        return View("AllWordsList", PaginatedList<Word>.Create(words, pageNumber ?? 1, 100));
    }
}