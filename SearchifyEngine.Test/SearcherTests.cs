using System.Threading.Tasks;
using NUnit.Framework;
using SearchifyEngine.Store;

namespace SearchifyEngine.Test
{
    public class SearcherTests
    {
        private Indexer.Indexer SetupIndexer()
        {
            IStore store = new InvertedIndexMemoryStore();
            var indexer = new Indexer.Indexer(store);
            indexer.Index("http://www.africau.edu/images/default/sample.pdf", 1);
            return indexer;
        }
        
        [Test]
        public async Task TestSearchReturns()
        {
            var indexer = SetupIndexer();
            var searcher = new Searcher.Searcher(indexer);
            var results = await searcher.ExecuteQuery("pdf");
            CollectionAssert.IsNotEmpty(results);
        }

        [Test]
        public async Task TestSearchNoResults()
        {
            var indexer = SetupIndexer();
            var searcher = new Searcher.Searcher(indexer);
            var results = await searcher.ExecuteQuery("balabala");
            CollectionAssert.IsEmpty(results);
        }
    }
}