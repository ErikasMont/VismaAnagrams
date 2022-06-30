using System.Text.Json;
using Cli.Helpers;
using Contracts.Interfaces;
using Contracts.Models;

namespace Cli;

public class UIWithEvents
{
    private readonly IWordService _wordService;
    private readonly AnagramApiClient _anagramsAnagramApi;
    private int _anagramsCount;
    private int _minLength;
    private readonly Func<string, string> _capitalizeFirstLetterDelegate;
    
    public EventHandler<string> PrintEvent; 
    public UIWithEvents(Func<string, string> capitalizeFirstLetter, IWordService wordService,
        int anagramsCount, int minLength, string anagramApiUrl)
    {
        _wordService = wordService;
        _anagramsCount = anagramsCount;
        _minLength = minLength;
        _anagramsAnagramApi = new AnagramApiClient(anagramApiUrl);
        _capitalizeFirstLetterDelegate = capitalizeFirstLetter;
    }

    public async Task RunAsync()
    {
        var flag = true;
        PrintEvent(this, "Hello! Welcome to anagram solver app");
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
        FormattedPrint(_capitalizeFirstLetterDelegate, anagramsString);
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
        PrintEvent(this, input);
    }
}