using System.Text.RegularExpressions;
using Contracts.Interfaces;
using Contracts.Models;

namespace BusinessLogic.Services;

public class AnagramSolver : IAnagramSolver
{
    private readonly IWordService _wordService;

    public AnagramSolver(IWordService wordService)
    {
        _wordService = wordService;
    }
    public List<Anagram> GetAnagrams(string myWords, int numberOfAnagrams)
    {
        var anagrams = new List<Anagram>();
        var anagram = new Anagram(myWords);
        myWords = Regex.Replace(myWords, @"\s+", "");
        var sortedWords = _wordService.GetSortedWords();
        if (sortedWords == null)
        {
            return anagrams;
        }

        if (myWords.Length == 0)
        {
            return anagrams;
        }
        var parts = anagram.Word.Split(' ');
        if (parts.Contains(""))
        {
            parts = parts.Where(x => x != "").ToArray();
        }
        if (parts.Length > 1)
        {
            var inputWords = parts.Select(x => new Anagram(x)).ToList();

            anagrams = GetSentenceAnagrams(sortedWords, inputWords, myWords, numberOfAnagrams);
            
            return anagrams;
        }

        anagram.Word = parts[0];
        var inputKey = _wordService.Alphabetize(myWords);

        if (!sortedWords.TryGetValue(inputKey, out anagrams))
        {
            return anagrams;
        }
        
        if (anagrams.Count == 0)
        {
            return anagrams;
        }
            
        var filteredAnagrams = _wordService.RemoveDuplicates(anagrams, anagram);
        if (filteredAnagrams == null)
        {
            return new List<Anagram>();
        }
            
        if (filteredAnagrams.Count == 0)
        {
            return filteredAnagrams;
        }
        if (filteredAnagrams.Count < numberOfAnagrams)
        {
            numberOfAnagrams = filteredAnagrams.Count;
        }
                
        return filteredAnagrams.GetRange(0, numberOfAnagrams);
    }

    private List<Anagram> GetSentenceAnagrams(Dictionary<string, List<Anagram>> sortedWords, List<Anagram> inputWords, 
        string myWords, int numberOfAnagrams)
    {
        var anagram = new Anagram(myWords);

        var sortedWord = _wordService.Alphabetize(myWords);
        var keys = sortedWords.Keys.ToList();

        var sentenceContainingKeys = FindSentenceContainingKeys(keys, sortedWord);

        var sentenceAnagramKeys = FindSentenceAnagramKeys(sentenceContainingKeys, sortedWord, inputWords.Count);

        var anagrams = FindAnagramSentences(sentenceAnagramKeys, sortedWords, inputWords);

        anagrams = _wordService.RemoveDuplicates(anagrams, anagram);

        if (anagrams == null)
        {
            return new List<Anagram>();
        }
        
        if (anagrams.Count < numberOfAnagrams)
        {
            numberOfAnagrams = anagrams.Count;
        }
            
        return anagrams.GetRange(0, numberOfAnagrams);
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
                if (!temp.Contains(key[i]))
                {
                    continue;
                }
                
                containedLetters++;
                var regex = new Regex(Regex.Escape(key[i].ToString()));
                temp = regex.Replace(temp, "", 1);
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
                var combination = utilList.Aggregate("", (current, key) => current + key.Word);
                combination += sentenceContainingKeys[i].Word;
                combination = _wordService.Alphabetize(combination);
                if (combination.Length != sortedWord.Length || !sortedWord.Equals(combination))
                {
                    continue;
                }
                var sentence = utilList.ToList();
                sentence.Add(sentenceContainingKeys[i]);
                sentenceAnagramKeys.Add(sentence);
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
            AnagramSentencesSearch(anagrams, pair, sortedWords,
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
                    sentence.Word += anagram.Word + " ";
                }
                
                if (!sortedWords.TryGetValue(keySentence[i].Word, out var values))
                {
                    continue;
                }
                
                values = values.Where(x => inputWords.Contains(x) == false).Distinct().ToList();
                if (values.Count == 0)
                {
                    continue;
                }
                
                foreach (var value in values)
                {
                    sentence.Word += value.Word;
                    anagrams.Add(sentence);
                }
            }
        }
        else
        {
            for (var i = currentIndex; i < keySentence.Count; i++)
            {
                if (!sortedWords.TryGetValue(keySentence[i].Word, out var values))
                {
                    continue;
                }
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