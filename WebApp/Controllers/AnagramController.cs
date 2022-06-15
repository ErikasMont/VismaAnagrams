using System.Text.Json;
using Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp.Helpers;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AnagramController : ControllerBase
{
    private readonly IAnagramSolver _anagramSolver;
    private readonly WordSettings _wordSettings;

    public AnagramController(IAnagramSolver anagramSolver, IOptions<WordSettings> wordSettings)
    {
        _anagramSolver = anagramSolver;
        _wordSettings = wordSettings.Value;
    }
    
    [HttpGet]
    public IActionResult GetAnagrams(string input)
    {
        if (input.Length < _wordSettings.MinInputLength)
        {
            return BadRequest("Input was too short.");
        }
        
        var anagrams = _anagramSolver.GetAnagrams(input, _wordSettings.AnagramCount);
        if (anagrams.Count == 0)
        {
            return NotFound("No anagrams found with given input.");
        }
        
        var anagramsJson = JsonSerializer.Serialize(anagrams);
        return Ok(anagramsJson);
    }
}