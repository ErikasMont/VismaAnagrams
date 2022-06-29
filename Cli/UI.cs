using System.Text.Json;
using System.Text.RegularExpressions;
using BusinessLogic.Services;
using Cli.Helpers;
using Contracts.Interfaces;
using Contracts.Models;

namespace Cli;

public class UI
{
    private readonly IWordService _wordService;
    private readonly AnagramApiClient _anagramsAnagramApi;
    private int _anagramsCount;
    private int _minLength;
    private delegate void Print(string message);

    private readonly Print _printDelegate;
    public UI(Action<string> writeToConsole, IWordService wordService,
        int anagramsCount, int minLength, string anagramApiUrl)
    {
        _wordService = wordService;
        _anagramsCount = anagramsCount;
        _minLength = minLength;
        _anagramsAnagramApi = new AnagramApiClient(anagramApiUrl);
        _printDelegate = new Print(writeToConsole);
    }

    public async Task RunAsync()
    {
        var flag = true;
        _printDelegate("Hello! Welcome to anagram solver app");
        var transfer = TrasferChoice();
        if (transfer)
        {
            await _wordService.WriteWordsToDb();
        }
        
        while (flag)
        {
            await UserInputAsync();
            flag = ExitChoice();
            if (!flag)
            {
                Console.WriteLine("Exiting.");
            }
        }
    }

    private async Task UserInputAsync()
    {
        Console.WriteLine("To find anagrams for a sentence separate the words using a space e.g (labas rytas)");
        Console.WriteLine("Please type in the words that you would like to get an anagram for: ");
        var word = Console.ReadLine();
        if (string.IsNullOrEmpty(word))
        {
            Console.WriteLine("No word given. Try again.");
            return;
        }

        var responseBody = await _anagramsAnagramApi.GetAnagramsRequestAsync(word);
        var anagrams = new List<Anagram>();
        try
        {
            anagrams = JsonSerializer.Deserialize<List<Anagram>>(responseBody);
        }
        catch (Exception ex)
        {
            Console.WriteLine(responseBody);
            return;
        }
        
        var anagramsString = StringifyAnagrams(anagrams);
        FormattedPrint(CapitalizeFirstLetter, anagramsString);
    }

    private bool ExitChoice()
    {
        Console.WriteLine("Would you like to exit? (y/n): ");
        var choice = Console.ReadLine();

        return choice switch
        {
            "n" => true,
            _ => false
        };
    }

    private bool TrasferChoice()
    {
        Console.WriteLine("Transfer files to database? (y/n)");
        var choice = Console.ReadLine();

        return choice switch
        {
            "y" => true,
            _ => false
        };
    }

    private string StringifyAnagrams(List<Anagram> anagrams)
    {
        return anagrams.Aggregate("", (current, anagram) => current + (anagram.Word + " "));
    }

    private void FormattedPrint(Func<string, string> capitalizeFirstLetter, string input)
    {
        input = capitalizeFirstLetter(input);
        _printDelegate(input);
    }
    
    private string CapitalizeFirstLetter(string input)
    {
        return Regex.Replace(input, "^[a-z]", m => m.Value.ToUpper());
    }
}