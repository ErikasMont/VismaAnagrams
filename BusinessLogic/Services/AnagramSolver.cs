using System.Text.RegularExpressions;
using Contracts.Interfaces;
using Contracts.Models;

namespace BusinessLogic.Services;

public class AnagramSolver : IAnagramSolver
{
    private readonly IWordRepository _wordRepository;

    public AnagramSolver(IWordRepository wordRepository)
    {
        _wordRepository = wordRepository;
    }
    public List<Anagram> GetAnagrams(string myWords, int numberOfAnagrams)
    {
        var anagram = new Anagram(myWords);
        myWords = Regex.Replace(myWords, @"\s+", "");
        var words = _wordRepository.ReadWords();
        var sortedWords = SortWords(words);
        var anagrams = new List<Anagram>();
        
        if (myWords.Length == 0)
        {
            return anagrams;
        }
        var parts = anagram.Word.Split(' ');
        if (parts.Length > 1)
        {
            var inputWords = new List<Anagram>();
            for (var i = 0; i < parts.Length; i++)
            { 
                inputWords.Add(new Anagram(parts[i]));
            }
            
            var sortedWord = Alphabetize(myWords);
            var keys = sortedWords.Keys.ToList();

            var sentenceContainingKeys = FindSentenceContainingKeys(keys, sortedWord);

            var sentenceAnagramKeys = FindSentenceAnagramKeys(sentenceContainingKeys, sortedWord, inputWords.Count);

            anagrams = FindAnagramSentences(sentenceAnagramKeys, sortedWords, inputWords);

            anagrams = RemoveDuplicates(anagrams, anagram);

            if (anagrams.Count < numberOfAnagrams)
            {
                numberOfAnagrams = anagrams.Count;
            }
            
            return anagrams.GetRange(0, numberOfAnagrams);
        }

        var inputKey = Alphabetize(myWords);

        if (sortedWords.TryGetValue(inputKey, out anagrams))
        {
            if (anagrams.Count == 0)
            {
                return anagrams;
            }
            
            var filteredAnagrams = RemoveDuplicates(anagrams, anagram);
            
            if (filteredAnagrams.Count == 0)
            {
                return filteredAnagrams;
            }
            else
            {
                if (anagrams.Count < numberOfAnagrams)
                {
                    numberOfAnagrams = anagrams.Count;
                }
                
                return filteredAnagrams.GetRange(0, numberOfAnagrams);
            }
        }
        
        return anagrams;
    }

    private Dictionary<string, List<Anagram>> SortWords(List<Word> words)
    {
        var sortedDictionary = new Dictionary<string, List<Anagram>>();
        foreach (var word in words)
        {
            var anagram = new Anagram(word.Value);
            var letters = word.Value.ToCharArray();
            Array.Sort(letters);
            var sortedKey = new string(letters);

            if (sortedDictionary.ContainsKey(sortedKey))
            {
                sortedDictionary[sortedKey].Add(anagram);
            }
            else
            {
                var anagrams = new List<Anagram>();
                anagrams.Add(anagram);
                sortedDictionary.Add(sortedKey, anagrams);
            }
        }
        
        return sortedDictionary;
    }

    private List<Anagram> RemoveDuplicates(List<Anagram> anagrams, Anagram userAnagram)
    {
        var filteredAnagrams = anagrams.Distinct().ToList();
        
        if (filteredAnagrams.Contains(userAnagram))
        {
            filteredAnagrams.Remove(userAnagram);
        }
        
        return filteredAnagrams;
    }

    private string Alphabetize(string value)
    {
        var letters = value.ToCharArray();
        Array.Sort(letters);
        var key = new string(letters);

        return key;
    }

    private List<Anagram> FindSentenceContainingKeys(List<string> keys, string sortedWord)
    {
        var sentenceContainingKeys = new List<Anagram>();
        foreach (var key in keys)
        {
            var keyLetters = key.ToCharArray();
            var temp = sortedWord;
            var containedLetters = 0;
            for (var i = 0; i < keyLetters.Length; i++)
            {
                if (temp.Contains(key[i]))
                {
                    containedLetters++;
                    var regex = new Regex(Regex.Escape(key[i].ToString()));
                    temp = regex.Replace(temp, "", 1);
                }
            }

            if (containedLetters == key.Length)
            {
                sentenceContainingKeys.Add(new Anagram(key));
            }
        }

        return sentenceContainingKeys;
    }

    private List<List<Anagram>> FindSentenceAnagramKeys(List<Anagram> sentenceContainingKeys, string sortedWord, int wordsCount)
    {
        var utilList = new List<Anagram>();
        var sentenceAnagramKeys = new List<List<Anagram>>();
        SentenceKeysSearch(sentenceContainingKeys, sentenceAnagramKeys, utilList,
            0, 0, wordsCount, sortedWord);

        return sentenceAnagramKeys;
    }

    private void SentenceKeysSearch(List<Anagram> sentenceContainingKeys, List<List<Anagram>> sentenceAnagramKeys,
        List<Anagram> utilList, int currentIndex, int currentLoop, int loops, string sortedWord)
    {
        if (currentLoop == loops - 1)
        {
            for (var i = currentIndex; i < sentenceContainingKeys.Count; i++)
            {
                string combination = "";
                foreach (var key in utilList)
                {
                    combination += key.Word;
                }
                combination += sentenceContainingKeys[i].Word;
                combination = Alphabetize(combination);
                if (combination.Length == sortedWord.Length && sortedWord.Equals(combination))
                {
                    var sentence = new List<Anagram>();
                    foreach (var key in utilList)
                    {
                        sentence.Add(key);
                    }
                    sentence.Add(sentenceContainingKeys[i]);
                    sentenceAnagramKeys.Add(sentence);
                }
            }
        }
        else
        {
            for (var i = currentIndex; i < sentenceContainingKeys.Count - loops + 1; i++)
            {
                utilList.Add(sentenceContainingKeys[i]);
                SentenceKeysSearch(sentenceContainingKeys, sentenceAnagramKeys, utilList, 
                    ++i, ++currentLoop, loops, sortedWord);
                i--;
                utilList.Remove(sentenceContainingKeys[i]);
                currentLoop--;
            }
        }
    }

    private List<Anagram> FindAnagramSentences(List<List<Anagram>> correctAnagrams,
        Dictionary<string, List<Anagram>> sortedWords, List<Anagram> inputWords)
    {
        var anagrams = new List<Anagram>();
        foreach (var pair in correctAnagrams)
        {
            var utilList = new List<Anagram>();
            var keySentence = pair;
            AnagramSentencesSearch(anagrams, keySentence, sortedWords,
                inputWords, utilList, 0, 0, pair.Count);
        }

        return anagrams;
    }

    private void AnagramSentencesSearch(List<Anagram> anagrams, List<Anagram> keySentence, 
        Dictionary<string, List<Anagram>> sortedWords, List<Anagram> inputWords, List<Anagram> utilList,
        int currentIndex, int currentLoop, int loops)
    {
        if (currentLoop == loops - 1)
        {
            for (var i = currentIndex; i < keySentence.Count; i++)
            {
                var sentence = new Anagram("");
                foreach (var anagram in utilList)
                {
                    sentence.Word = sentence.Word + " " + anagram.Word;
                }
                var values = new List<Anagram>();
                if (sortedWords.TryGetValue(keySentence[i].Word, out values))
                {
                    values = values.Where(x => inputWords.Contains(x) == false).Distinct().ToList();
                    if (values.Count == 0)
                    {
                        continue;
                    }
                    foreach (var value in values)
                    {
                        sentence.Word = sentence.Word + " " + value.Word;
                        anagrams.Add(sentence);
                    }
                }
            }
        }
        else
        {
            for (var i = currentIndex; i < keySentence.Count; i++)
            {
                var values = new List<Anagram>();
                if (sortedWords.TryGetValue(keySentence[i].Word, out values))
                {
                    values = values.Where(x => inputWords.Contains(x) == false).ToList();
                    if (values.Count == 0)
                    {
                        continue;
                    }
                    foreach (var value in values)
                    {
                        utilList.Add(value);
                        AnagramSentencesSearch(anagrams, keySentence, sortedWords, inputWords,
                            utilList, ++i, ++currentLoop, loops);
                        i--;
                        utilList.Remove(value);
                        currentLoop--;
                    }
                }
            }
        }
    }
}