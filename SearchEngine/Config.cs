
using System;
using System.IO;
using InvertedTomato.Compression.Integers;

namespace SearchEngine
{
    /// <summary>
    /// Application Constants
    /// </summary>
    public static class Config
    {
        public static string AppDataDirectory = Environment.GetEnvironmentVariable("SEARCH_ENGINE_DATA_DIR") ?? "/home/oluwafenyi/CSC-322 Assignments/SearchEngineData/";
        public static readonly string IndexFilePath = Path.Combine(AppDataDirectory, "ReverseIndex.bin");
        public static readonly string IndexJsonPath = Path.Combine(AppDataDirectory, "ReverseIndex.json");
        public static readonly Codec Codec = new FibonacciCodec();
    }
}