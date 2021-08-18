using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using ProtoBuf;
using TikaOnDotNet.TextExtraction;

namespace SearchEngine.Indexer
{

    /// <summary>
    /// Indexer class builds and maintains internal search index
    /// </summary>
    [ProtoContract]
    public class Indexer
    {
        /// <summary>
        /// Last File ID indexed
        /// </summary>
        [ProtoMember(1)]
        public uint LastId = 0;
        private TextExtractor _textExtractor = new TextExtractor();
        
        /// <summary>
        /// Internal Inverted Index representation, maps a string (word) to a list of Index Terms
        /// </summary>
        [DataMember, ProtoMember(2)]
        public Dictionary<string, List<IndexTerm>> ReverseIndex = new Dictionary<string, List<IndexTerm>>();

        /// <summary>
        /// Serializes and dumps indexer to a binary file specified at path: <see cref="Config.IndexFilePath"/>
        /// </summary>
        public void DumpIndex()
        {
            using (var file = File.Create(Config.IndexFilePath))
            {
                Serializer.Serialize(file, this);
            }
        }

        /// <summary>
        /// Serializes and dumps index to a json file specified at path <see cref="Config.IndexJsonPath"/>
        /// </summary>
        public void DumpJson()
        {
            using (StreamWriter file = File.CreateText(Config.IndexJsonPath))
            {
                string json = JsonConvert.SerializeObject(ReverseIndex, Formatting.None, new IndexTermJsonConverter());
                file.WriteLine(json);
            }
        }

        /// <summary>
        /// Initializes a new Indexer object from existing index binary file at path: <see cref="Config.IndexFilePath"/>
        /// </summary>
        /// <returns>Indexer instance with same state as in file</returns>
        public static Indexer LoadIndex()
        {
            Indexer indexer;
            using (FileStream file = File.OpenRead(Config.IndexFilePath)) {
                indexer = Serializer.Deserialize<Indexer>(file);
            }

            return indexer;
        }
        
        // initializes and appends index term to internal index
        private void _indexWord(string word, uint fileId, List<int> positions)
        {
            IndexTerm indexTerm;
            if (!ReverseIndex.ContainsKey(word))
            {
                ReverseIndex[word] = new List<IndexTerm>();
            }
            indexTerm = new IndexTerm(fileId);
            List<ulong> deltaArrayList = Utils.ToDeltaList(positions);
            indexTerm.AddPositions(deltaArrayList.ToArray());
            ReverseIndex[word].Add(indexTerm);
        }

        /// <summary>
        /// Returns index list associated with <paramref name="word"/>
        /// </summary>
        /// <param name="word">any string</param>
        /// <returns>Index list of word</returns>
        public IndexTerm[] GetIndexTermArray(string word)
        {
            List<IndexTerm> indexList;
            bool ok = ReverseIndex.TryGetValue(word, out indexList);
            if (ok)
            {
                return indexList.ToArray();
            }

            return Array.Empty<IndexTerm>();
        }

        // sums up filedeltas in a termlist
        private uint SumDeltasInTermList(string word)
        {
            return (uint) GetIndexTermArray(word).Sum(indexTerm => indexTerm.FileDeltaToUint());
        }

        /// <summary>
        /// Powerhouse function for indexing documents
        /// </summary>
        /// <param name="filePath">a path or link to an indexable document</param>
        /// <param name="fileId">unique integer id for document</param>
        public void Index(string filePath, uint fileId)
        {
            // local map of word to positions
            Dictionary<string, List<int>> map = new Dictionary<string, List<int>>();
            TextExtractionResult content = _textExtractor.Extract(filePath);

            // generate list of relevant words for indexing
            string[] tokens = Tokenizer.Tokenizer.Tokenize(content.Text);

            int pos = 0;
            
            // iterate over tokens and store their positions in local map
            foreach (var token in tokens)
            {
                pos++;
                
                if (!map.ContainsKey(token))
                {
                    map.Add(token, new List<int>());
                }
                
                map[token].Add(pos);
            }

            // iterate over local map and index word, delta and positions
            foreach (var word in map.Keys)
            {
                uint delta = fileId - SumDeltasInTermList(word);
                _indexWord(word, delta, map[word]);
            }

            LastId = fileId;
        }
    }
}