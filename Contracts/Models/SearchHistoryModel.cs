namespace Contracts.Models;

public class SearchHistoryModel
{
    public string UserIP { get; set; }
    public DateTime SearchDate { get; set; }
    public string SearchString { get; set; }
    public string FoundAnagrams { get; set; }

    public SearchHistoryModel(string userIp, DateTime searchDate, string searchString, string foundAnagrams)
    {
        UserIP = userIp;
        SearchDate = searchDate;
        SearchString = searchString;
        FoundAnagrams = foundAnagrams;
    }
}