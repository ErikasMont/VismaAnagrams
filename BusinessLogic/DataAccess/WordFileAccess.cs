using System.Data;
using System.Data.SqlClient;
using BusinessLogic.Helpers;
using Contracts.Models;
using Contracts.Interfaces;
using Microsoft.Extensions.Options;

namespace BusinessLogic.DataAccess;

public class WordFileAccess : IWordRepository
{
    private const string FileName = "zodynas.txt";
    private readonly ConnectionStrings _connectionStrings;

    public WordFileAccess(IOptions<ConnectionStrings> connectionStrings)
    {
        _connectionStrings = connectionStrings.Value;
    }
    
    public List<Word> ReadWords()
    {
        var words = new List<Word>();
        try
        {
            using (var fs = File.Open(FileName, FileMode.Open, FileAccess.Read))
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
        }
        catch (Exception ex)
        {
            
        }
        return words;
    }

    public void WriteWord(Word word)
    {
        try
        {
            using (var fs = File.Open(FileName, FileMode.Append, FileAccess.Write))
            {
                using (var bs = new BufferedStream(fs))
                {
                    using (var sw = new StreamWriter(bs))
                    {
                        sw.WriteLine(word.Value);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            
        }
    }

    public void TransferWords()
    {
        var words = ReadWords().Distinct().ToList();
        var sql = "INSERT INTO dbo.word (value) VALUES (@param);";

        try
        {
            using (var connection = new SqlConnection(_connectionStrings.AnagramsDb))
            {
                connection.Open();
                var count = GetWordsCountFromDb();
                if (count != 0)
                {
                    return;
                }
            
                foreach (var word in words)
                {
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@param", SqlDbType.VarChar, 50).Value = word.Value;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            
        }
    }

    private int GetWordsCountFromDb()
    {
        var sql = "SELECT COUNT(id) FROM dbo.word;";

        try
        {
            using (var connection = new SqlConnection(_connectionStrings.AnagramsDb))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    var count = (int)command.ExecuteScalar();
                    return count;
                }
            }
        }
        catch (Exception ex)
        {
            return -1;
        }
    }
}