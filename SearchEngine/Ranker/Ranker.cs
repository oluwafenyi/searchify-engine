using System;
using System.Collections.Generic;
using System.Linq;
using SearchEngine.Searcher;

namespace SearchEngine.Ranker
{
    
    /// <summary>
    /// Maintains and calculates document scores for a query
    /// </summary>
    public class Ranker
    {
        private Indexer.Indexer _indexer;
        private Dictionary<uint, double> _scores = new Dictionary<uint, double>();

        /// <summary>
        /// Instantiates a ranker object
        /// </summary>
        /// <param name="indexer">an instance of <see cref="Indexer.Indexer"/></param>
        public Ranker(Indexer.Indexer indexer)
        {
            _indexer = indexer;
        }

        /// <summary>
        /// Computes and stores file score
        /// </summary>
        /// <param name="fieldId">id of file</param>
        /// <param name="pointerList">pointer list of query terms that can be found in the file</param>
        public void Score(uint fieldId, List<Pointer> pointerList)
        {
            _tfIdfScore(fieldId, pointerList);
        }

        /// <summary>
        /// Returns an ordered array of file ids based on scores
        /// </summary>
        /// <returns>list of file ids</returns>
        public uint[] RankedResultsList()
        {
            string text = "";
            foreach (KeyValuePair<uint, double> kvp in _scores)
            {
                text += $"Key = {kvp.Key}, Value = {kvp.Value}\n";
            }
            Console.WriteLine(text);
            
            return _scores.OrderByDescending(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .ToArray();
        }
        
        // TF IDF Scoring
        private void _tfIdfScore(uint fileId, List<Pointer> pointerList)
        {
            double total = 0;
            foreach (var pointer in pointerList)
            {
                // normalized term frquency: number of occurences of term in document
                double tf = _indexer.GetIndexTermArray(pointer.Term)[pointer.P].Frequency;
                
                // inverse document frquency: lg(N / df(t))
                // N : total number of documents
                // df(t): total number of documents that have term t
                double idf = Math.Log(_indexer.LastId / (double) _indexer.GetIndexTermArray(pointer.Term).Length, 2);
                
                total += tf * idf;
            }
            _scores.Add(fileId, total);
        }
    }
}