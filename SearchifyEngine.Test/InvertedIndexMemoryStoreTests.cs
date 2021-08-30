using System.Threading.Tasks;
using NUnit.Framework;
using SearchifyEngine.Indexer;
using SearchifyEngine.Store;

namespace SearchifyEngine.Test
{
    public class InvertedIndexMemoryStoreTests
    {
        [Test]
        public async Task TestInvertedIndexGetLastIdDefault()
        {
            IStore store = new InvertedIndexMemoryStore();
            Assert.AreEqual(0, await store.GetLastId());
        }

        [Test]
        public async Task TestInvertedIndexSetLastId()
        {
            IStore store = new InvertedIndexMemoryStore();
            await store.SetLastId(50);
            Assert.AreEqual(50, await store.GetLastId());
        }

        [Test]
        public async Task TestCheckTermIndexed()
        {
            IStore store = new InvertedIndexMemoryStore();
            Assert.IsFalse(await store.CheckTermIndexed("test"));
            await store.AppendIndexTerm("test", new IndexTerm(1));
            Assert.IsTrue(await store.CheckTermIndexed("test"));
        }

        [Test]
        public async Task TestAppendIndexTerm()
        {
            IStore store = new InvertedIndexMemoryStore();
            var term = new IndexTerm(1);
            term.AddPositions(new uint[]{1, 2, 3, 4, 5});
            await store.AppendIndexTerm("test2", term);
            var termList = await store.GetIndexTermList("test2");
            Assert.AreEqual(term, termList[0]);
            CollectionAssert.AreEqual(term.Positions, termList[0].Positions);
        }

        [Test]
        public async Task TestMultipleAppendIndexTerm()
        {
            IStore store = new InvertedIndexMemoryStore();
            await store.AppendIndexTerm("test3", new IndexTerm(1));
            await store.AppendIndexTerm("test3", new IndexTerm(1));
            await store.AppendIndexTerm("test3", new IndexTerm(1));
            await store.AppendIndexTerm("test3", new IndexTerm(1));
            await store.AppendIndexTerm("test3", new IndexTerm(1));
            await store.AppendIndexTerm("test3", new IndexTerm(1));

            var list = await store.GetIndexTermList("test3");
            Assert.AreEqual(6, list.Count);
        }
    }
}