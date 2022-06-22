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
    private const string WordCacheTableName = "cachedword";
    private const string SearchHistoryTableName = "searchhistory";

    public WordDbAccess(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("AnagramsDb") ??
                            throw new Exception("Connection string was not found.");
    }

    public async Task<IEnumerable<WordModel>> ReadWords()
    {
        var words = new List<WordModel>();
        var sql = $"SELECT value FROM {WordTableName}";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            var wordFromDb = reader[0].ToString();
            var word = new WordModel(wordFromDb);
            words.Add(word);
        }

        return words;
    }

    public async Task WriteWord(WordModel wordModel)
    {
        var sql = $"INSERT INTO {WordTableName} VALUES(@param)";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@param", SqlDbType.VarChar, 50).Value = wordModel.Value;
        command.CommandType = CommandType.Text;
        await command.ExecuteNonQueryAsync();
    }

    public async Task WriteWords(IEnumerable<WordModel> words)
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

    public async Task AddToCache(WordModel wordModel, string anagrams)
    {
        var sql = $"INSERT INTO {WordCacheTableName} VALUES(@word, @anagrams)";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@word", SqlDbType.VarChar, 50).Value = wordModel.Value;
        command.Parameters.Add("@anagrams", SqlDbType.VarChar, 250).Value = anagrams;
        command.CommandType = CommandType.Text;
        await command.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<CachedWordModel>> ReadWordsFromCache()
    {
        var words = new List<CachedWordModel>();
        var sql = $"SELECT word, anagrams FROM {WordCacheTableName}";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            var word = reader[0].ToString();
            var anagrams = reader[1].ToString();
            var cachedWord = new CachedWordModel(word, anagrams);
            words.Add(cachedWord);
        }

        return words;
    }

    public async Task RemoveWordFromCache(WordModel wordModel)
    {
        var spName = "spRemoveCachedWord";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(spName, connection);
        command.Parameters.Add("@givenWord", SqlDbType.VarChar, 50).Value = wordModel.Value;
        command.CommandType = CommandType.StoredProcedure;
        await command.ExecuteNonQueryAsync();
    }

    public async Task AddToSearchHistory(SearchHistoryModel model)
    {
        var sql = $"INSERT INTO {SearchHistoryTableName} VALUES(@userip, @searchdate, @searchstring, @foundanagrams)";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@userip", SqlDbType.VarChar, 50).Value = model.UserIP;
        command.Parameters.Add("@searchdate", SqlDbType.DateTime).Value = model.SearchDate;
        command.Parameters.Add("@searchstring", SqlDbType.VarChar, 50).Value = model.SearchString;
        command.Parameters.Add("@foundanagrams", SqlDbType.VarChar, 250).Value = model.FoundAnagrams;
        command.CommandType = CommandType.Text;
        await command.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<WordModel>> SearchWordsByFilter(string input)
    {
        var foundWords = new List<WordModel>();
        var sql = $"SELECT value FROM {WordTableName} WHERE value LIKE @input";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@input", SqlDbType.VarChar).Value = $"'%{input}%'";
        await using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            var wordFromDb = reader[0].ToString();
            var word = new WordModel(wordFromDb);
            foundWords.Add(word);
        }

        return foundWords;
    }
}