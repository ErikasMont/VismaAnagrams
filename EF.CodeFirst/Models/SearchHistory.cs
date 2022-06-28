namespace EF.CodeFirst.Models;

public class SearchHistory
{
    public int Id { get; set; }
    public string UserIp { get; set; }
    public DateTime SearchDate { get; set; }
    public string SearchString { get; set; }
    public string? FoundAnagrams { get; set; }
}