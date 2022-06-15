using System.Text.Json;
using Cli.Helpers;
using Contracts.Interfaces;
using Contracts.Models;

namespace Cli;

public class UI
{
    private IAnagramSolver _anagramSolver;
    private readonly ApiHelper _anagramsApi;
    private int _anagramsCount;
    private int _minLength;
    public UI(IAnagramSolver anagramSolver, int anagramsCount, int minLength)
    {
        _anagramSolver = anagramSolver;
        _anagramsCount = anagramsCount;
        _minLength = minLength;
        _anagramsApi = new ApiHelper();
    }

    public async Task Run()
    {
        var flag = true;
        Console.WriteLine("Hello! Welcome to anagram solver app");
        while (flag)
        {
            await UserInput();
            flag = ExitChoice();
            if (!flag)
            {
                Console.WriteLine("Exiting.");
            }
        }
    }

    private async Task UserInput()
    {
        Console.WriteLine("To find anagrams for a sentence separate the words using a space e.g (labas rytas)");
        Console.WriteLine("Please type in the words that you would like to get an anagram for: ");
        var word = Console.ReadLine();
        if (word == null)
        {
            Console.WriteLine("No word given. Try again.");
        }

        var responseBody = await _anagramsApi.GetAnagramsRequest(word);
        try
        {
            var anagrams = JsonSerializer.Deserialize<List<Anagram>>(responseBody);
            PrintAnagrams(anagrams);
        }
        catch (Exception ex)
        {
            Console.WriteLine(responseBody);
        }
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

    private void PrintAnagrams(List<Anagram> anagrams)
    {
        Console.Write("Anagrams: ");
        foreach (var anagram in anagrams)
        {
            Console.Write("{0} ", anagram.Word);
        }
        Console.WriteLine();
    }
}