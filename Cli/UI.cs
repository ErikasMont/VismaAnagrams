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
        Console.WriteLine("To find anagrams for a sentence separate the words using a space e.g (labas rytas)");
        Console.WriteLine("Please type in the words that you would like to get an anagram for: ");
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
                Console.WriteLine("No anagrams were found for the given words.");
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
        Anagram test = new Anagram("keras vaga");
        if (anagrams.Contains(test))
        {
            Console.WriteLine("praeina");
        }
        foreach (var anagram in anagrams)
        {
            Console.Write("{0} ", anagram.Word);
        }
        Console.WriteLine();
    }
    
}