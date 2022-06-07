namespace Contracts.Models;

public class Word
{
    public string Value { get; set; }

    public Word(string word)
    {
        Value = word;
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
}