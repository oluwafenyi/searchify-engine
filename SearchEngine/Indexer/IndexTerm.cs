using System.IO;
using ProtoBuf;

namespace SearchEngine.Indexer
{
    /// <summary>
    /// Index term representation, stores file delta, frequency and positions of word
    /// </summary>
    [ProtoContract]
    public class IndexTerm
    {
        // internal rep of fileDelta
        private MemoryStream _fileDeltaStream = new MemoryStream();
        
        // internal rep of positions array
        private MemoryStream _positionsStream = new MemoryStream();
        
        /// <summary>
        /// Number of times word appears in file
        /// </summary>
        [ProtoMember(3)]
        public uint Frequency;

        // required for serialization
        public IndexTerm()
        {
            
        }

        /// <summary>
        /// Difference in id between this file and previous file in list of index terms
        /// </summary>
        /// <param name="fileDelta">nonnegative integer</param>
        public IndexTerm(uint fileDelta)
        {
            // uses integer compression to encode fileDelta, compressed value is stored in _fileDeltaStream
            Config.Codec.EncodeSingle(_fileDeltaStream, fileDelta);
        }
        
        /// <summary>
        /// Sets list of positions for term
        /// </summary>
        /// <param name="positions">list of positions where term can be found in the file</param>
        public void AddPositions(ulong[] positions)
        {
            Frequency = (uint) positions.Length;
            // uses integer compression to encode list of positions
            Config.Codec.EncodeMany(_positionsStream, positions);
        }
        
        /// <summary>
        /// Getter and Setter methods for FileDelta, to be used implicitly by serializer only
        /// </summary>
        [ProtoMember(4)]
        protected byte[] FileDelta
        {
            get
            {
                _fileDeltaStream.Seek(0, SeekOrigin.Begin);
                return _fileDeltaStream.ToArray();
            }
            set => _fileDeltaStream = new MemoryStream(value);
        }

        /// <summary>
        /// Getter and Setter methods for Positions, to be used implicitly by serializer only
        /// </summary>
        [ProtoMember(5)]
        protected byte[] Positions
        {
            get
            {
                _positionsStream.Seek(0, SeekOrigin.Begin);
                return _positionsStream.ToArray();
            }
            set => _positionsStream = new MemoryStream(value);
        }

        /// <summary>
        /// Returns FileDelta as uint
        /// </summary>
        /// <returns>nonnegative file delta value</returns>
        public uint FileDeltaToUint()
        {
            _fileDeltaStream.Seek(0, SeekOrigin.Begin);
            return (uint) Config.Codec.DecodeSingle(_fileDeltaStream);           
        }

        /// <summary>
        /// Returns of word positions as ulong array
        /// </summary>
        /// <returns>array of positions</returns>
        public ulong[] PositionsToUlongArray()
        {
            ulong[] values = new ulong[Frequency];
            _positionsStream.Seek(0, SeekOrigin.Begin);
            Config.Codec.DecodeMany(_positionsStream, values);
            return values;
        }
    }
}