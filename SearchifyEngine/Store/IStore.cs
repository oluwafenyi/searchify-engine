using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using SearchifyEngine.Indexer;

namespace SearchifyEngine.Store
{
    /// <summary>
    /// Defines methods that must be possessed by an Inverted Index Store
    /// </summary>
    public interface IStore
    {
        
        /// <summary>
        /// Sets the value of the last document indexed
        /// </summary>
        /// <param name="lastId">document id</param>
        /// <returns>status code for operation</returns>
        Task<HttpStatusCode> SetLastId(uint lastId);
        
        /// <summary>
        /// Returns the id of last file indexed, zero if no file was indexed.
        /// </summary>
        /// <returns>id of last file indexed</returns>
        Task<uint> GetLastId();
        
        /// <summary>
        /// Appends to list of index terms for a particular term. If the term has not been indexed yet, a new list is
        /// instantiated and the term is then appended
        /// </summary>
        /// <param name="term">term</param>
        /// <param name="indexTerm"><see cref="IndexTerm"/> object</param>
        /// <returns>status code of operation</returns>
        Task<HttpStatusCode> AppendIndexTerm(string term, IndexTerm indexTerm);

        
        /// <summary>
        /// Checks if a term has been indexed
        /// </summary>
        /// <param name="term">term</param>
        /// <returns>true if term has been indexed, else false</returns>
        Task<bool> CheckTermIndexed(string term);

        /// <summary>
        /// Returns index term list for a particular term. An empty list is returned if the term has not been indexed
        /// </summary>
        /// <param name="term">term</param>
        /// <returns>list of <see cref="IndexTerm"/> objects</returns>
        Task<List<IndexTerm>> GetIndexTermList(string term);
    }
}