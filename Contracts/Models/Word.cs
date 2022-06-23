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
    
    public override int GetHashCode()
    {
        int hashWord = Value == null ? 0 : Value.GetHashCode();
        return hashWord;
    }
}