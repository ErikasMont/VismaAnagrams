using Contracts.Models;

namespace WebApp.Models;

public class AnagramViewModel
{
    public string SearchString { get; set; }
    public List<Anagram> Anagrams { get; set; }
    public string ErrorMessage { get; set; }
    
}