using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using SearchifyEngine.Indexer;

namespace SearchifyEngine.Store
{
    public class InvertedIndexMemoryStore: IStore
    {
        private uint _lastId = 0;

        private Dictionary<string, List<IndexTerm>> _invertedIndex = new Dictionary<string, List<IndexTerm>>();

        /// <summary>
        /// Returns the id of last file indexed, zero if no file was indexed.
        /// </summary>
        /// <returns>id of last file indexed</returns>
        public async Task<uint> GetLastId()
        {
            return _lastId;
        }

        /// <summary>
        /// Sets the value of the last document indexed
        /// </summary>
        /// <param name="lastId">document id</param>
        /// <returns>status code for operation</returns>
        public async Task<HttpStatusCode> SetLastId(uint lastId)
        {
            _lastId = lastId;
            return HttpStatusCode.OK;
        }
        
        /// <summary>
        /// Appends to list of index terms for a particular term. If the term has not been indexed yet, a new list is
        /// instantiated and the term is then appended
        /// </summary>
        /// <param name="term">term</param>
        /// <param name="indexTerm"><see cref="IndexTerm"/> object</param>
        /// <returns>status code of operation</returns>
        public async Task<HttpStatusCode> AppendIndexTerm(string term, IndexTerm indexTerm)
        {
            if (!_invertedIndex.ContainsKey(term))
            {
                _invertedIndex[term] = new List<IndexTerm>();
            }
            _invertedIndex[term].Add(indexTerm);

            return HttpStatusCode.OK;
        }

        /// <summary>
        /// Checks if a term has been indexed
        /// </summary>
        /// <param name="term">term</param>
        /// <returns>true if term has been indexed, else false</returns>
        public async Task<bool> CheckTermIndexed(string term)
        {
            return _invertedIndex.ContainsKey(term);
        }

        /// <summary>
        /// Returns index term list for a particular term. An empty list is returned if the term has not been indexed
        /// </summary>
        /// <param name="term">term</param>
        /// <returns>list of <see cref="IndexTerm"/> objects</returns>
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