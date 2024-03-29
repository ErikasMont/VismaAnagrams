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

    public async Task<IEnumerable<Word>> ReadWords()
    {
        var words = new List<Word>();
        var sql = $"SELECT value FROM {WordTableName}";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            var wordFromDb = reader[0].ToString();
            var word = new Word(wordFromDb);
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

    public async Task AddToCache(Word word, string anagrams)
    {
        var sql = $"INSERT INTO {WordCacheTableName} VALUES(@word, @anagrams)";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@word", SqlDbType.VarChar, 50).Value = word.Value;
        command.Parameters.Add("@anagrams", SqlDbType.VarChar, 250).Value = anagrams;
        command.CommandType = CommandType.Text;
        await command.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<CachedWord>> ReadWordsFromCache()
    {
        var words = new List<CachedWord>();
        var sql = $"SELECT word, anagrams FROM {WordCacheTableName}";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            var word = reader[0].ToString();
            var anagrams = reader[1].ToString();
            var cachedWord = new CachedWord(word, anagrams);
            words.Add(cachedWord);
        }

        return words;
    }

    public async Task RemoveWordFromCache(Word word)
    {
        var spName = "spRemoveCachedWord";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(spName, connection);
        command.Parameters.Add("@givenWord", SqlDbType.VarChar, 50).Value = word.Value;
        command.CommandType = CommandType.StoredProcedure;
        await command.ExecuteNonQueryAsync();
    }

    public async Task AddToSearchHistory(SearchHistory model)
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

    public async Task<IEnumerable<Word>> SearchWordsByFilter(string input)
    {
        var foundWords = new List<Word>();
        var sql = $"SELECT value FROM {WordTableName} WHERE value LIKE @input";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@input", SqlDbType.VarChar).Value = $"'%{input}%'";
        await using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            var wordFromDb = reader[0].ToString();
            var word = new Word(wordFromDb);
            foundWords.Add(word);
        }

        return foundWords;
    }

    public Task RemoveWord(string word)
    {
        throw new NotImplementedException();
    }

    public Task EditWord(string existingWord, string editedWord)
    {
        throw new NotImplementedException();
    }

    public Task Commit()
    {
        throw new NotImplementedException();
    }
}