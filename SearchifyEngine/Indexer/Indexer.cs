using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SearchifyEngine.Store;
using TikaOnDotNet.TextExtraction;

namespace SearchifyEngine.Indexer
{

    /// <summary>
    /// Indexer class builds and maintains internal search index
    /// </summary>
    public class Indexer
    {

        /// <summary>
        /// Last File ID indexed
        /// </summary>
        public uint LastId;
        
        // tika extractor
        private readonly TextExtractor _textExtractor = new TextExtractor();

        // index store
        private readonly IStore _store;

        // local reverse index representation for queries
        private readonly Dictionary<string, IndexTerm[]> _reverseIndex = new Dictionary<string, IndexTerm[]>();
        
        /// <summary>
        /// Instantiates an Indexer object
        /// </summary>
        /// <param name="store">object that implements <see cref="IStore"/>></param>
        public Indexer(IStore store)
        {
            _store = store;
            Task.Run(async () =>
            {
                LastId = await store.GetLastId();
            }).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Caches terms from store
        /// </summary>
        /// <param name="terms">array of terms</param>
        public async Task LoadInvertedIndex(string[] terms)
        {
            await Task.WhenAll(terms.Select(async t =>
            {
                IndexTerm[] indexTermList = await GetIndexTermArray(t);
                _reverseIndex.Add(t, indexTermList);
                return indexTermList;
            }));
        }

        /// <summary>
        /// Returns array of index terms from cache. If term hasn't been cached, an empty array is returned.
        /// </summary>
        /// <param name="term">term</param>
        /// <returns>array of index terms</returns>
        public IndexTerm[] GetLoadedTermList(string term)
        {
            try
            {
                return _reverseIndex[term];
            }
            catch (KeyNotFoundException)
            {
                return Array.Empty<IndexTerm>();
            }
        }

        // initializes and appends index term to internal index
        private async Task IndexWord(string word, uint fileId, List<uint> positions)
        {
            var indexTerm = new IndexTerm(fileId);
            List<uint> deltaArrayList = Utils.ToDeltaList(positions);
            indexTerm.AddPositions(deltaArrayList.ToArray());
            await _store.AppendIndexTerm(word, indexTerm);
        }
        
        // gets index term list from store
        private async Task<IndexTerm[]> GetIndexTermArray(string word)
        {
            List<IndexTerm> list = await _store.GetIndexTermList(word);
            return list.ToArray();
        }

        // sums up filedeltas in a termlist
        private async Task<uint> SumDeltasInTermList(string word)
        {
            var terms = await GetIndexTermArray(word);
            return (uint) terms.Sum(indexTerm => indexTerm.FileDelta);
        }

        /// <summary>
        /// Powerhouse function for indexing documents
        /// </summary>
        /// <param name="fileUrl">a path or link to an indexable document</param>
        /// <param name="fileId">unique integer id for document</param>
        public async Task Index(string fileUrl, uint fileId)
        {
            string filePath = ExtractDoc.Extract(fileUrl);

            if (filePath == null)
            {
                Console.WriteLine("error: error getting file at -> " + fileUrl);
                return;
            }
            
            // local map of word to positions
            Dictionary<string, List<uint>> map = new Dictionary<string, List<uint>>();
            TextExtractionResult content = _textExtractor.Extract(filePath);

            // generate list of relevant words for indexing
            string[] tokens = Tokenizer.Tokenizer.Tokenize(content.Text);

            uint pos = 0;
            
            // iterate over tokens and store their positions in local map
            foreach (var token in tokens)
            {
                pos++;
                
                if (!map.ContainsKey(token))
                {
                    map.Add(token, new List<uint>());
                }
                
                map[token].Add(pos);
            }
            
            // iterate over local map and index word, delta and positions
            foreach (var word in map.Keys)
            {
                uint total = await SumDeltasInTermList(word);
                uint delta = fileId - total;
                await IndexWord(word, delta, map[word]);
            }

            ExtractDoc.Delete(filePath);
            LastId = fileId;
            await _store.SetLastId(fileId);
        }
    }
}