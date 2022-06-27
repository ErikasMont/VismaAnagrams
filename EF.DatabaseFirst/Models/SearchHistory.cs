using System;
using System.Collections.Generic;

namespace EF.DatabaseFirst.Models
{
    public partial class SearchHistory
    {
        public int Id { get; set; }
        public string UserIp { get; set; } = null!;
        public DateTime SearchDate { get; set; }
        public string SearchString { get; set; } = null!;
        public string? FoundAnagrams { get; set; }

        public SearchHistory(string userIp, DateTime searchDate, string searchString, string foundAnagrams)
        {
            UserIp = userIp;
            SearchDate = searchDate;
            SearchString = searchString;
            FoundAnagrams = foundAnagrams;
        }
    }
}
