using Contracts.Models;
using Contracts.Interfaces;

namespace BusinessLogic.DataAccess;

public class WordDataAccess : IWordRepository
{
    private const string _fileName = "zodynas.txt";
    public List<Word> ReadWords()
    {
        var words = new List<Word>();
        using (FileStream fs = File.Open(_fileName, FileMode.Open, FileAccess.Read))
        {
            using (BufferedStream bs = new BufferedStream(fs))
            {
                using (StreamReader sr = new StreamReader(bs))
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
}