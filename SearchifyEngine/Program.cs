using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SearchifyEngine.Database;
using SearchifyEngine.Store;

namespace SearchifyEngine
{
    static class Program
    {
        private static string _directory = "";
        
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                await MainAsync(args);
            }).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            DbClient.CreateClient(true);
            await DbClient.CreateTables();
            IStore store = DbClient.Store;

            // Console.WriteLine(InvertedIndex.CheckTermIndexed("assign"));
            // uint lastId = await InvertedIndex.GetLastId();
            // Console.WriteLine(lastId);
            // Indexer.Indexer indexer = new Indexer.Indexer(lastId);
            // foreach (var indexTerm in indexer.GetIndexTermArray("assign"))
            // {
            //     Console.WriteLine(indexTerm.FileDelta);
            // }

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Indexer.Indexer indexer = await LoadIndex(store);
            stopwatch.Stop();
            
            Console.WriteLine("Took " + stopwatch.ElapsedMilliseconds + "ms to load the index");
            
            Searcher.Searcher searcher = new Searcher.Searcher(indexer);
            
            stopwatch.Reset();
            stopwatch.Start();
            var fileIds = await searcher.ExecuteQuery("finite state machines");
            stopwatch.Stop();
            
            Console.WriteLine("FileIds: " + string.Join(" ", fileIds));
            Console.WriteLine("Query took " +  stopwatch.ElapsedMilliseconds + "ms");
        }

        private static async Task<Indexer.Indexer> BuildIndex(IStore store)
        {
            var indexer = new Indexer.Indexer(store);
            string[] files = Directory.GetFiles(_directory)
                .OrderBy(f => f).ToArray();
            uint id = 1;
            var stopwatch = new Stopwatch();
            foreach (var file in files)
            {
                Console.WriteLine("Indexing " + file);
                stopwatch.Start();
                await indexer.Index(file, id);
                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds);
                stopwatch.Reset();
                id++;
            }
            
            return indexer;
        }

        private static async Task<Indexer.Indexer> LoadIndex(IStore store)
        {
            Indexer.Indexer indexer;
            uint lastId = await store.GetLastId();
            uint fileCount = (uint) Directory.GetFiles(_directory).Length;
            if (lastId != fileCount)
            {
                indexer = await BuildIndex(store);
            }
            else
            {
                indexer = new Indexer.Indexer(store);
            }
            
            return indexer;
        }
    }
}