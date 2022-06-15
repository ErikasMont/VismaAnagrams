using Contracts.Models;
using Contracts.Interfaces;

namespace BusinessLogic.DataAccess;

public class WordDataAccess : IWordRepository
{
    private const string _fileName = "zodynas.txt";
    public List<Word> ReadWords()
    {
        var words = new List<Word>();
        using (var fs = File.Open(_fileName, FileMode.Open, FileAccess.Read))
        {
            using (var bs = new BufferedStream(fs))
            {
                using (var sr = new StreamReader(bs))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                        var value = parts[0];
                        var word = new Word(value);
                        words.Add(word);
                    }
                }
            }
        }
        return words;
    }

    public void WriteWords(List<Word> words)
    {
        using (var fs = File.Open(_fileName, FileMode.Create, FileAccess.Write))
        {
            using (var bs = new BufferedStream(fs))
            {
                using (var sw = new StreamWriter(bs))
                {
                    foreach (var word in words)
                    {
                        sw.WriteLine(word.Value);
                    }
                }
            }
        }
    }
}