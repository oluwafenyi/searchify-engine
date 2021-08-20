using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SearchEngine.Database;
using SearchEngine.Database.Models;

namespace SearchEngine
{
    static class Program
    {
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
            Indexer.Indexer indexer = await LoadIndex();
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

        private static async Task<Indexer.Indexer> BuildIndex()
        {
            uint lastId = await InvertedIndex.GetLastId();
            var indexer = new Indexer.Indexer(lastId);
            string[] files = Directory.GetFiles(Path.Combine(Config.AppDataDirectory, "repository"))
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

        private static async Task<Indexer.Indexer> LoadIndex()
        {
            Indexer.Indexer indexer;
            uint lastId = await InvertedIndex.GetLastId();
            uint fileCount = (uint) Directory.GetFiles(Path.Combine(Config.AppDataDirectory, "repository")).Length;
            if (lastId != fileCount)
            {
                indexer = await BuildIndex();
            }
            else
            {
                indexer = new Indexer.Indexer(lastId);
            }
            
            return indexer;
        }
    }
}