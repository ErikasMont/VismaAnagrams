using System;
using System.Collections.Generic;

namespace EF.DatabaseFirst.Models
{
    public partial class Word
    {
        public int Id { get; set; }
        public string Value { get; set; } = null!;

        public Word(string value)
        {
            Value = value;
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Word))
            {
                return false;
            }
            Word anagram = obj as Word;

            return this.Value.Equals(anagram.Value);
        }
    
        public override int GetHashCode()
        {
            int hashWord = Value == null ? 0 : Value.GetHashCode();
            return hashWord;
        }
    }
}
