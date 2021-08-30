using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SearchifyEngine.Indexer;
using SearchifyEngine.Store;
using TikaOnDotNet.TextExtraction;

namespace SearchifyEngine.Test
{
    public class IndexerTests
    {
        private Indexer.Indexer SetupIndexer()
        {
            IStore store = new InvertedIndexMemoryStore();
            var indexer = new Indexer.Indexer(store);
            indexer.Index("http://www.africau.edu/images/default/sample.pdf", 1);
            return indexer;
        }
        
        [Test]
        public async Task TestIndexing()
        {
            IStore store = new InvertedIndexMemoryStore();
            Indexer.Indexer indexer = new Indexer.Indexer(store);
            
            var url = "http://www.africau.edu/images/default/sample.pdf";
            var filePath = ExtractDoc.Extract(url);
            var text = new TextExtractor().Extract(filePath).Text;
            var tokens = Tokenizer.Tokenizer.Tokenize(text);
            
            await indexer.Index(url, 1);

            var list = new List<bool>(); 
            foreach (var s in new HashSet<string>(tokens))
            {
                list.Add(await store.CheckTermIndexed(s));
            }

            bool status = list.AsQueryable().All(val => val);
            Assert.IsTrue(status);
        }

        [Test]
        public async Task TestLoadingCache()
        {
            var indexer = SetupIndexer();
            await indexer.LoadInvertedIndex(new string[] { "pdf" });
            CollectionAssert.IsNotEmpty(indexer.GetLoadedTermList("pdf"));
        }
    }
}