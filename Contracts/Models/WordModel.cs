namespace Contracts.Models;

public class WordModel
{
    public string Value { get; set; }

    public WordModel(string word)
    {
        Value = word;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is WordModel))
        {
            return false;
        }
        WordModel anagram = obj as WordModel;

        return this.Value.Equals(anagram.Value);
    }
    
    public override int GetHashCode()
    {
        int hashWord = Value == null ? 0 : Value.GetHashCode();
        return hashWord;
    }
}