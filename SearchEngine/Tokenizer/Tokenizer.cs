using System.Collections.Generic;
using System.Linq;
using StopWord;

namespace SearchEngine.Tokenizer
{
    
    /// <summary>
    /// Static class that houses tokenization logic
    /// </summary>
    public static class Tokenizer
    {
        private static readonly Stemmer Stemmer = new Stemmer();
        private static readonly HashSet<string> StopWordsSet = new HashSet<string>(StopWords.GetStopWords("en"));

        /// <summary>
        /// Tokenizes text
        /// </summary>
        /// <param name="text">any string value</param>
        /// <returns>array of stemmed words with stopwords filtered out</returns>
        public static string[] Tokenize(string text)
        {
            string cleanedText = Utils.CleanText(text);
            var tokens = cleanedText.Split(null)
                .Where(word => !StopWordsSet.Contains(word) && word != "")
                .Select(word => Stemmer.StemWord(word));
            return tokens.ToArray();
        }
    }
}