using System;
using System.Collections.Generic;

namespace EF.DatabaseFirst.Models
{
    public partial class Word
    {
        public int Id { get; set; }
        public string Value { get; set; } = null!;

        public virtual CachedWord CachedWord { get; set; } = null!;

        public Word(string value)
        {
            Value = value;
        }
    }
}
