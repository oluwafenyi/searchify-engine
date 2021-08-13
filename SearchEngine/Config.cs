
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
        public static string HomeDirectory = Environment.GetEnvironmentVariable("SEARCH_ENGINE_HOME") ?? "/home/oluwafenyi/CSC-322 Assignments/SearchEngine/SearchEngine/";
        public static readonly string IndexFilePath = Path.Combine(HomeDirectory, "ReverseIndex.bin");
        public static readonly string IndexJsonPath = Path.Combine(HomeDirectory, "ReverseIndex.json");
        public static readonly Codec Codec = new FibonacciCodec();
    }
}