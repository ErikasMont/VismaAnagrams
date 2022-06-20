using System.Data;
using System.Data.SqlClient;
using Contracts.Interfaces;
using Contracts.Models;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic.DataAccess;

public class WordDbAccess : IWordRepository
{
    private readonly string _connectionString;
    private const string WordTableName = "word";

    public WordDbAccess(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("AnagramsDb") ??
                            throw new Exception("Connection string was not found.");
    }

    public async Task<IEnumerable<Word>> ReadWords()
    {
        var words = new List<Word>();
        var sql = $"SELECT value FROM {WordTableName}";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var word = new Word(reader[0].ToString());
            words.Add(word);
        }

        return words;
    }

    public async Task WriteWord(Word word)
    {
        var sql = $"INSERT INTO {WordTableName} VALUES(@param)";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@param", SqlDbType.VarChar, 50).Value = word.Value;
        command.CommandType = CommandType.Text;
        await command.ExecuteNonQueryAsync();
    }

    public async Task WriteWords(IEnumerable<Word> words)
    {
        var sql = $"INSERT INTO {WordTableName} VALUES(@param)";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        foreach (var word in words)
        {
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.Add("@param", SqlDbType.VarChar, 50).Value = word.Value;
            command.CommandType = CommandType.Text;
            await command.ExecuteNonQueryAsync();
        }
    }
}