using Contracts.Models;

namespace WebApp.Models;

public class SearchWordViewModel
{
    public string SearchString { get; set; }
    public List<Word> Words { get; set; }
    public string ErrorMessage { get; set; }
}