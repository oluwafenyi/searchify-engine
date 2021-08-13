using System;
using System.Diagnostics;
using System.IO;

namespace SearchEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            Indexer.Indexer indexer = new Indexer.Indexer();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            indexer.Index(Path.Combine(Config.HomeDirectory, "repository/file_0000.txt"), 0);
            stopwatch.Stop();
            indexer.Index(Path.Combine(Config.HomeDirectory, "repository/file_0001.txt"), 1); 

            var stopwatch0 = new Stopwatch();
            stopwatch0.Start();
            indexer.DumpIndex();
            stopwatch0.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            Console.WriteLine(stopwatch0.ElapsedMilliseconds);
            
            indexer.DumpJson();
        }
    }
}