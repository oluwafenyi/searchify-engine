using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SearchEngine.Indexer
{
    /// <summary>
    /// Custom converter class to aid the serialization of <see cref="IndexTerm"/> to JSON
    /// <example>
    /// string json = JsonConvert.SerializeObject(ReverseIndex, Formatting.None, new IndexTermJsonConverter());
    /// </example>
    /// </summary>
    public class IndexTermJsonConverter : JsonConverter<IndexTerm>
    {
        // serialization
        public override void WriteJson(JsonWriter writer, IndexTerm term, JsonSerializer serializer)
        {
            uint fileDelta = term.FileDelta;
            uint[] positions = term.Positions;
            
            // serialized IndexTerm as a JS Object with the fields fileDelta, positions and frequency
            serializer.Serialize(writer, new Dictionary<string, object>
            {
                {"fileDelta", fileDelta},
                {"positions", positions},
                {"frequency", term.Frequency}
            });
        }
        
        // deserialization
        public override IndexTerm ReadJson(JsonReader reader, Type objectType, IndexTerm existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            Dictionary<string, object> dict = serializer.Deserialize<Dictionary<string, object>>(reader);
            object obj;
            uint delta = 0;
            uint[] positions = new uint[] { };
            if (dict != null)
            {
                dict.TryGetValue("fileDelta", out obj);
                if (obj != null)
                {
                    delta = (uint) obj;
                }

                dict.TryGetValue("positions", out obj);
                if (obj != null)
                {
                    positions = (uint[]) obj;
                }
            }
            IndexTerm indexTerm = new IndexTerm(delta);
            indexTerm.AddPositions(positions);
            return indexTerm;
        }
    }
}