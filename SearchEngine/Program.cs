using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SearchEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Indexer.Indexer indexer = LoadIndex();
            stopwatch.Stop();
            
            Console.WriteLine("Took " + stopwatch.ElapsedMilliseconds + "ms to load the index");
            
            Searcher.Searcher searcher = new Searcher.Searcher(indexer);
            
            stopwatch.Reset();
            stopwatch.Start();
            var fileIds = searcher.ExecuteQuery("finite state machines");
            stopwatch.Stop();
            
            Console.WriteLine("FileIds: " + string.Join(" ", fileIds));
            Console.WriteLine("Query took " +  stopwatch.ElapsedMilliseconds + "ms");
        }

        private static Indexer.Indexer BuildIndex()
        {
            var indexer = new Indexer.Indexer();
            string[] files = Directory.GetFiles(Path.Combine(Config.AppDataDirectory, "repository"))
                .OrderBy(f => f).ToArray();
            uint id = 1;
            var stopwatch = new Stopwatch();
            foreach (var file in files)
            {
                Console.WriteLine("Indexing " + file);
                stopwatch.Start();
                indexer.Index(file, id);
                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds);
                stopwatch.Reset();
                id++;
            }
            indexer.DumpIndex();
            indexer.DumpJson();
            return indexer;
        }

        private static Indexer.Indexer LoadIndex()
        {
            Indexer.Indexer indexer;
            if (File.Exists(Config.IndexFilePath))
            {
                indexer = Indexer.Indexer.LoadIndex();
                string[] files = Directory.GetFiles(Path.Combine(Config.AppDataDirectory, "repository"))
                    .OrderBy(f => f).ToArray();
                if (indexer.LastId != files.Length)
                {
                    indexer = BuildIndex();
                }
            }
            else
            {
                indexer = BuildIndex();
            }
            return indexer;
        }
    }
}