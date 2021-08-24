using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using SearchifyEngine.Indexer;

namespace SearchifyEngine.Store
{
    public interface IStore
    {
        Task<HttpStatusCode> SetLastId(uint id);
        Task<uint> GetLastId();
        Task<HttpStatusCode> AppendIndexTerm(string term, IndexTerm indexTerm);

        Task<bool> CheckTermIndexed(string term);

        Task<List<IndexTerm>> GetIndexTermList(string term);
    }
}