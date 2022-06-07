using Contracts.Interfaces;
using Contracts.Models;

namespace Cli;

public class UI
{
    private IAnagramSolver _anagramSolver;
    private int _anagramsCount;
    private int _minLength;
    public UI(IAnagramSolver anagramSolver, int anagramsCount, int minLength)
    {
        _anagramSolver = anagramSolver;
        _anagramsCount = anagramsCount;
        _minLength = minLength;
    }

    public void Run()
    {
        var flag = true;
        Console.WriteLine("Hello! Welcome to anagram solver app");
        while (flag)
        {
            UserInput();
            flag = ExitChoice();
        }
    }

    private void UserInput()
    {
        Console.WriteLine("Please type in the word that you would like to get an anagram for: ");
        var word = Console.ReadLine();
        if (string.IsNullOrEmpty(word))
        {
            Console.WriteLine("No word given. Try again.");
        }
        else if(word.Length < _minLength)
        {
            Console.WriteLine("The word was too short. You need to enter a word that has at least {0} characters.", _minLength);
        }
        else
        {
            var anagrams = _anagramSolver.GetAnagrams(word, _anagramsCount);
            if (anagrams == null || anagrams.Count == 0)
            {
                Console.WriteLine("No anagrams were found for the given word.");
            }
            else
            {
                PrintAnagrams(anagrams);
            }
        }
    }

    private bool ExitChoice()
    {
        Console.WriteLine("Would you like to exit? (y/n): ");
        var choice = Console.ReadLine();
        if (string.IsNullOrEmpty(choice) || (choice != "y" && choice != "n"))
        {
            Console.WriteLine("Wrong input given. Exiting.");
            return false;
        }
        else if(choice == "y")
        {
            Console.WriteLine("Exiting.");
            return false;
        }
        else
        {
            return true;
        }
    }

    private void PrintAnagrams(List<Anagram> anagrams)
    {
        Console.Write("Anagrams: ");
        foreach (var anagram in anagrams)
        {
            Console.Write("{0} ", anagram.Word);
        }
    }
    
}