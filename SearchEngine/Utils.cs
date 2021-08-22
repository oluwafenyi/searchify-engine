using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SearchEngine
{
    /// <summary>
    /// Static class housing a number of utility methods
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Converts multiple spaces to one an strips punctuation from text, converts text to lowercase
        /// </summary>
        /// <param name="text">any string value</param>
        /// <returns>cleaned text</returns>
        public static string CleanText(string text)
        {
            text = Regex.Replace(text, "[^A-Za-z0-9 ]", " ").ToLower();
            text = Regex.Replace(text, @"\s+", " ");
            return text;
        }

        /// <summary>
        /// Creates a list where elements are replaced by the value of the delta between each element and the previous element
        /// </summary>
        /// <param name="list">list of nonnegative integers</param>
        /// <returns>list of delta ulong values</returns>
        public static List<uint> ToDeltaList(List<uint> list)
        {
            List<uint> output = new List<uint>();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                {
                    output.Add(list[i]);
                    continue;
                }
                output.Add(list[i] - list[i - 1]);
            }

            return output;
        }
    }
}