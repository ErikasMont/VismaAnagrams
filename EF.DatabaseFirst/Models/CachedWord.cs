using System;
using System.Collections.Generic;

namespace EF.DatabaseFirst.Models
{
    public partial class CachedWord
    {
        public int Id { get; set; }
        public string? Anagrams { get; set; }
        public int FkWordId { get; set; }

        public virtual Word FkWord { get; set; } = null!;

        public CachedWord(string anagrams, int fkWordId)
        {
            Anagrams = anagrams;
            FkWordId = fkWordId;
        }
    }
}
