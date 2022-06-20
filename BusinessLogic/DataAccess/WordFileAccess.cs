using Contracts.Models;
using Contracts.Interfaces;

namespace BusinessLogic.DataAccess;

public class WordFileAccess : IWordRepository
{
    private const string FileName = "zodynas.txt";

    public async Task<IEnumerable<Word>> ReadWords()
    {
        var words = new List<Word>();
        await using var fs = File.Open(FileName, FileMode.Open, FileAccess.Read);
        await using var bs = new BufferedStream(fs);
        using var sr = new StreamReader(bs);
        string line;
        while ((line = await sr.ReadLineAsync()) != null)
        {
            var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
            var word = new Word(parts[0]);
            words.Add(word);
        }

        return words;
    }

    public async Task WriteWord(Word word)
    {
        await using var fs = File.Open(FileName, FileMode.Append, FileAccess.Write);
        await using var bs = new BufferedStream(fs);
        await using var sw = new StreamWriter(bs);
        await sw.WriteLineAsync(word.Value);
    }

    public async Task WriteWords(IEnumerable<Word> words)
    {
        await using var fs = File.Open(FileName, FileMode.Append, FileAccess.Write);
        await using var bs = new BufferedStream(fs);
        await using var sw = new StreamWriter(bs);
        foreach (var word in words)
        {
            await sw.WriteLineAsync(word.Value);
        }
    }
}