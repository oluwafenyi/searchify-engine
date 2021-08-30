using System;
using InvertedTomato.Compression.Integers;

namespace SearchifyEngine
{
    /// <summary>
    /// Application Constants
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Dynamo database host
        /// </summary>
        public static readonly string DatabaseHost = Environment.GetEnvironmentVariable("DYNAMO_DATABASE_HOST") ?? "localhost";
        
        /// <summary>
        /// Dynamo database port
        /// </summary>
        public static readonly int DatabasePort = Environment.GetEnvironmentVariable("DYNAMO_DATABASE_PORT") != null ? Convert.ToInt32(Environment.GetEnvironmentVariable("DYNAMO_DATABASE_PORT")) : 8000;
        
        /// <summary>
        /// Codec for integer compression
        /// </summary>
        public static readonly Codec Codec = new FibonacciCodec();
    }
}