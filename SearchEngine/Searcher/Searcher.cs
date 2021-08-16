using System;
using System.Collections.Generic;
using MoreComplexDataStructures;

namespace SearchEngine.Searcher
{
    /// <summary>
    /// Searcher class operates on an indexer
    /// </summary>
    public class Searcher
    {
        private Indexer.Indexer _indexer;
        private Ranker.Ranker _ranker;

        /// <summary>
        /// Instantiates a Searcher object
        /// </summary>
        /// <param name="indexer">instance of <see cref="Indexer.Indexer"/></param>
        public Searcher(Indexer.Indexer indexer)
        {
            _indexer = indexer;
            _ranker = new Ranker.Ranker(_indexer);
        }

        /// <summary>
        /// Returns a ranked array of file ids associated with a query
        /// </summary>
        /// <param name="query">any nonempty string value</param>
        /// <returns>Ranked array of file ids</returns>
        public uint[] ExecuteQuery(string query)
        {
            string[] queryTerms = Tokenizer.Tokenizer.Tokenize(query);
            MinHeap<Pointer> heap = new MinHeap<Pointer>();

            // initialize pq
            foreach (var term in queryTerms)
            {
                heap.Insert(new Pointer(term, 0, _indexer.GetIndexTermArray(term)[0].FileDeltaToUint()));
            }
            
            while (heap.Count > 0)
            {
                // peek smallest id
                uint minFileId = heap.Peek().FileId;
            
                List<Pointer> currP = new List<Pointer>();
            
                // pop all pointers to id
                while (heap.Count > 0 && heap.Peek().FileId == minFileId)
                {
                    currP.Add(heap.ExtractMin());
                }
            
                // score
                _ranker.Score(minFileId, currP);
            
                // increment pointers and file ids and deltas
                foreach (var pointer in currP)
                {
                    try
                    {
                        var nextPointer = new Pointer(pointer.Term, pointer.P + 1,
                            _indexer.GetIndexTermArray(pointer.Term)[pointer.P + 1].FileDeltaToUint() + pointer.FileId);
                        heap.Insert(nextPointer);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                    }
                    catch (IndexOutOfRangeException)
                    {
                    }
                }
            }
            
            uint[] resultFileIds = _ranker.RankedResultsList();
            return resultFileIds;
        }
    }
}