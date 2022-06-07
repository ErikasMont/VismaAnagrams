using Contracts.Models;
using Contracts.Interfaces;

namespace BusinessLogic.Repositories;

public class WordRepository : IWordRepository
{
    private const string _fileName = "zodynas.txt";
    public Dictionary<string, List<Anagram>> ReadDictionary()
    {
        var anagrams = new Dictionary<string, List<Anagram>>();
        using (FileStream fs = File.Open(_fileName, FileMode.Open, FileAccess.Read))
        {
            using (BufferedStream bs = new BufferedStream(fs))
            {
                using (StreamReader sr = new StreamReader(bs))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                        string value = parts[0];
                        Anagram word = new Anagram(value);

                        char[] letters = value.ToCharArray();
                        Array.Sort(letters);
                        string key = new string(letters);

                        if (anagrams.ContainsKey(key))
                        {
                            if (!anagrams[key].Contains(word))
                            {
                                anagrams[key].Add(word);
                            }
                        }
                        else
                        {
                            List<Anagram> words = new List<Anagram>();
                            words.Add(word);
                            anagrams.Add(key, words);
                        }
                    }
                }
            }
        }
        return anagrams;
    }
}