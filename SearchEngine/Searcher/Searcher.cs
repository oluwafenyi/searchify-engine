using System;
using System.Collections.Generic;
using MoreComplexDataStructures;

namespace SearchEngine.Searcher
{
    public class Searcher
    {
        private Indexer.Indexer _indexer;
        private Dictionary<uint, double> scores = new Dictionary<uint, double>();

        public Searcher(Indexer.Indexer indexer)
        {
            _indexer = indexer;
        }

        public void ExecuteQuery(string query)
        {
            string[] queryTerms = Tokenizer.Tokenizer.Tokenize(query);
            int[] pointers = new int[queryTerms.Length];
            MinHeap<Pointer> heap = new MinHeap<Pointer>();

            // initialize pq
            foreach (var term in queryTerms)
            {
                heap.Insert(new Pointer(term, 0, _indexer.GetIndexList(term)[0].FileDeltaToUint()));
            }
            
            // peek smallest id
            uint minFileId = heap.Peek().FileId;
            
            List<Pointer> currP = new List<Pointer>();
            
            // pop all pointers to id
            while (heap.Peek().FileId == minFileId)
            {
                currP.Add(heap.ExtractMin());
            }
            
            _score(minFileId, currP);
            
            



        }

        // TF IDF Scoring
        private void _score(uint fileId, List<Pointer> pointerList)
        {
            double total = 0;
            foreach (var pointer in pointerList)
            {
                double tf = _indexer.GetIndexList(pointer.Term)[pointer.P].Frequency;
                double idf = Math.Log(_indexer.LastId / (double) _indexer.GetIndexList(pointer.Term).Length);
                total += tf * idf;
            }
            scores.Add(fileId, total);
        }
    }
}