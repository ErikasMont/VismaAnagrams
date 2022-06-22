namespace Contracts.Models;

public class AnagramModel
{
    public string Word { get; set; }

    public AnagramModel(string word)
    {
        Word = word;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is AnagramModel))
        {
            return false;
        }
        AnagramModel anagramModel = obj as AnagramModel;

        return this.Word.Equals(anagramModel.Word);
    }
    
    public override int GetHashCode()
    {
        int hashWord = Word == null ? 0 : Word.GetHashCode();
        return hashWord;
    }
}