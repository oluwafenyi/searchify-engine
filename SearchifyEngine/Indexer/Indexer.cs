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
        
        private TextExtractor _textExtractor = new TextExtractor();

        private IStore _store;

        public Dictionary<string, IndexTerm[]> ReverseIndex = new Dictionary<string, IndexTerm[]>();
        
        public Indexer(IStore store)
        {
            _store = store;
            Task.Run(async () =>
            {
                LastId = await store.GetLastId();
            }).GetAwaiter().GetResult();
        }

        public async Task LoadInvertedIndex(string[] queryTerms)
        {
            await Task.WhenAll(queryTerms.Select(async t =>
            {
                IndexTerm[] terms = await GetIndexTermArray(t);
                ReverseIndex.Add(t, terms);
                return terms;
            }));
        }

        public IndexTerm[] GetLoadedTermList(string term)
        {
            return ReverseIndex[term];
        }

        // initializes and appends index term to internal index
        private async Task _indexWord(string word, uint fileId, List<uint> positions)
        {
            IndexTerm indexTerm;
            indexTerm = new IndexTerm(fileId);
            List<uint> deltaArrayList = Utils.ToDeltaList(positions);
            indexTerm.AddPositions(deltaArrayList.ToArray());
            await _store.AppendIndexTerm(word, indexTerm);
        }

        /// <summary>
        /// Returns index list associated with <paramref name="word"/>
        /// </summary>
        /// <param name="word">any string</param>
        /// <returns>Index list of word</returns>
        public async Task<IndexTerm[]> GetIndexTermArray(string word)
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
        /// <param name="filePath">a path or link to an indexable document</param>
        /// <param name="fileId">unique integer id for document</param>
        public async Task Index(string filePath, uint fileId)
        {
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
                await _indexWord(word, delta, map[word]);
            }

            LastId = fileId;
            await _store.SetLastId(fileId);
        }
    }
}