using System.Data;
using System.Data.SqlClient;
using BusinessLogic.Helpers;
using Contracts.Interfaces;
using Contracts.Models;
using Microsoft.Extensions.Options;

namespace BusinessLogic.DataAccess;

public class WordDbAccess : IWordRepository
{
    private const string FileName = "zodynas.txt";
    private readonly ConnectionStrings _connectionStrings;

    public WordDbAccess(IOptions<ConnectionStrings> connectionStrings)
    {
        _connectionStrings = connectionStrings.Value;
    }
    
    public List<Word> ReadWords()
    {
        var words = new List<Word>();
        var sql = "SELECT value FROM dbo.word";

        try
        {
            using (var connection = new SqlConnection(_connectionStrings.AnagramsDb))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var word = new Word(reader[0].ToString());
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
        var sql = "INSERT INTO word VALUES(@param)";

        try
        {
            using (var connection = new SqlConnection(_connectionStrings.AnagramsDb))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@param", SqlDbType.VarChar, 50).Value = word.Value;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            
        }
    }

    public void TransferWords()
    {
        var words = ReadWords();

        try
        {
            using (var fs = File.Open(FileName, FileMode.Append, FileAccess.Write))
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
        catch (Exception ex)
        {
            
        }
    }
}