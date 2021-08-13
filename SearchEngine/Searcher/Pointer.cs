using System;

namespace SearchEngine.Searcher
{
    public class Pointer: IComparable, IComparable<Pointer>
    {
        public string Term;
        public uint P;
        public uint FileId;

        public Pointer(string term, uint p, uint fileId)
        {
            P = p;
            Term = term;
            FileId = fileId;
        }
        
        public int CompareTo(object other)
        {
            if (other == null)
            {
                throw new ArgumentException("cannot compare Pointer to null");
            }

            Pointer pointer = (Pointer) other;
            if (FileId == pointer.FileId) return 0;
            if (FileId < pointer.FileId) return -1;
            return 1;
        }

        public int CompareTo(Pointer other)
        {
            if (other.GetType() != typeof(Pointer))
            {
                throw new ArgumentException("cannot compare Pointer to " + other.GetType());
            }
            if (ReferenceEquals(this, other)) return 0;
            if (FileId == other.FileId) return 0;
            if (FileId < other.FileId) return -1;
            return 1;
        }
    }
}