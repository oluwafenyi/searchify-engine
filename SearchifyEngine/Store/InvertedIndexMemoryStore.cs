using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using SearchifyEngine.Indexer;

namespace SearchifyEngine.Store
{
    public class InvertedIndexMemoryStore: IStore
    {
        private uint LastId = 0;

        private Dictionary<string, List<IndexTerm>> _invertedIndex = new Dictionary<string, List<IndexTerm>>();

        public async Task<uint> GetLastId()
        {
            return LastId;
        }

        public async Task<HttpStatusCode> SetLastId(uint id)
        {
            LastId = id;
            return HttpStatusCode.OK;
        }

        public async Task<HttpStatusCode> AppendIndexTerm(string term, IndexTerm indexTerm)
        {
            if (!_invertedIndex.ContainsKey(term))
            {
                _invertedIndex[term] = new List<IndexTerm>();
            }
            _invertedIndex[term].Add(indexTerm);

            return HttpStatusCode.OK;
        }

        public async Task<bool> CheckTermIndexed(string term)
        {
            return _invertedIndex.ContainsKey(term);
        }

        public async Task<List<IndexTerm>> GetIndexTermList(string term)
        {
            if (await CheckTermIndexed(term))
            {
                return _invertedIndex[term];
            }

            return new List<IndexTerm>();
        }
    }
}