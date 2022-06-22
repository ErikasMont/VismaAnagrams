using System;
using System.Collections.Generic;

namespace EF.DatabaseFirst.Models
{
    public partial class CachedWord
    {
        public int Id { get; set; }
        public string Word { get; set; } = null!;
        public string? Anagrams { get; set; }

        public CachedWord(string word, string anagrams)
        {
            Word = word;
            Anagrams = anagrams;
        }
    }
}
