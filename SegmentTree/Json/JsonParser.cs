using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SegmentTree.Json
{
    public class ParserException : Exception
    {
    }

    public struct JsonSegment
    {
        static Memory<byte> MemoryFromStr(string src)
        {
            return Encoding.UTF8.GetBytes(src);
        }
        public static readonly Memory<byte> NULL_MEMORY = MemoryFromStr("null");
        public static readonly Memory<byte> TRUE_MEMORY = MemoryFromStr("true");
        public static readonly Memory<byte> FALSE_MEMORY = MemoryFromStr("false");

        public readonly Memory<byte> Buffer;

        public JsonSegment(Memory<byte> buffer)
        {
            Buffer = buffer;
        }

        public bool IsNull
        {
            get
            {
                var t = Buffer.Span;
                return t.SequenceEqual(NULL_MEMORY.Span);
            }
        }

        public bool GetBool()
        {
            var t = Buffer.Span;
            if (t.SequenceEqual(TRUE_MEMORY.Span))
            {
                return true;
            }
            else if (t.SequenceEqual(FALSE_MEMORY.Span))
            {
                return false;
            }
            else
            {
                throw new ParserException();
            }
        }
    }

    public class JsonParser
    {
        public JsonSegment Parse(Memory<byte> src)
        {
            return new JsonSegment(src);
        }
    }

    public static class JsonParserExtensions
    {
        public static JsonSegment Parse(this JsonParser p, string src)
        {
            var bytes = Encoding.UTF8.GetBytes(src);
            return p.Parse(bytes);
        }
    }
}