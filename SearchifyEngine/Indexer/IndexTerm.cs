using System.IO;
using System.Linq;

namespace SearchifyEngine.Indexer
{
    /// <summary>
    /// Index term representation, stores file delta, frequency and positions of word
    /// </summary>
    public class IndexTerm
    {
        
        // internal rep of fileDelta
        private readonly MemoryStream _frequencyStream = new MemoryStream();

        // internal rep of positions array
        private readonly MemoryStream _positionsStream = new MemoryStream();
        
        /// <summary>
        /// File ID delta value
        /// </summary>
        public readonly uint FileDelta;
        
        /// <summary>
        /// Instantiates a new IndexTerm object
        /// </summary>
        /// <param name="fileDelta">delta value</param>
        public IndexTerm(uint fileDelta)
        {
            FileDelta = fileDelta;
        }

        /// <summary>
        /// Sets positions for term
        /// </summary>
        /// <param name="positions">array of positions in delta uint array</param>
        public void AddPositions(uint[] positions)
        {
            Config.Codec.EncodeSingle(_frequencyStream, (ulong) positions.Length);
            Config.Codec.EncodeMany(_positionsStream, positions.Select(i => (ulong) i).ToArray());
        }
        
        /// <summary>
        /// Array of positions where term can be found in the document
        /// </summary>
        public uint[] Positions
        {
            get
            {
                ulong[] values = new ulong[Frequency];
                _positionsStream.Seek(0, SeekOrigin.Begin);
                Config.Codec.DecodeMany(_positionsStream, values);
                return values.Select(i => (uint) i).ToArray();
            }
        }

        /// <summary>
        /// Number of occurrences of term in document
        /// </summary>
        public uint Frequency
        {
            get
            {
                _frequencyStream.Seek(0, SeekOrigin.Begin);
                ulong value = Config.Codec.DecodeSingle(_frequencyStream);
                return (uint) value;
            }
        }
    }
}