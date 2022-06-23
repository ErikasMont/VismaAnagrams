namespace Contracts.Models;

public class Anagram
{
    public string Word { get; set; }

    public Anagram(string word)
    {
        Word = word;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Anagram))
        {
            return false;
        }
        Anagram anagram = obj as Anagram;

        return this.Word.Equals(anagram.Word);
    }
    
    public override int GetHashCode()
    {
        int hashWord = Word == null ? 0 : Word.GetHashCode();
        return hashWord;
    }
}