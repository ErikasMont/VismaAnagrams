using System.Text.Json;
using Cli.Helpers;
using Contracts.Interfaces;
using Contracts.Models;

namespace Cli;

public class UI : IDisplay
{
    private readonly IWordService _wordService;
    private readonly AnagramApiClient _anagramsAnagramApi;

    private readonly Action<string> _printDelegate;
    private readonly Func<string, string> _capitalizeFirstLetterDelegate;
    public UI(Action<string> print, Func<string, string> capitalizeFirstLetter, IWordService wordService, string anagramApiUrl)
    {
        _wordService = wordService;
        _anagramsAnagramApi = new AnagramApiClient(anagramApiUrl);
        _printDelegate = print;
        _capitalizeFirstLetterDelegate = capitalizeFirstLetter;
    }

    public async Task RunAsync()
    {
        var shouldExit = true;
        _printDelegate("Hello! Welcome to anagram solver app");
        var shouldTransfer = ShouldTransferDataToDb();
        if (shouldTransfer)
        {
            await _wordService.WriteWordsToDb();
        }
        
        while (shouldExit)
        {
            await UserInputAsync();
            shouldExit = ShouldExitApp();
            if (!shouldExit)
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

    private bool ShouldExitApp()
    {
        Console.WriteLine("Would you like to exit? (y/n): ");
        var choice = Console.ReadLine();

        return choice switch
        {
            "n" => true,
            _ => false
        };
    }

    private bool ShouldTransferDataToDb()
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
}