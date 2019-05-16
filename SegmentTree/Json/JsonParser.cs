using System;
using System.Buffers.Text;
using System.Runtime.InteropServices;
using System.Text;

namespace SegmentTree.Json
{
    public class JsonParseException : Exception
    {
    }

    public enum JsonValueType
    {
        Null,
        Boolean,

        Number,

        String,

        List,

        Object,
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

        public JsonValueType ValueType
        {
            get
            {
                var ch = Buffer.Span[0];
                switch (ch)
                {
                    case (byte)'n':
                        return JsonValueType.Null;
                    case (byte)'t':
                    case (byte)'f': // fall through
                        return JsonValueType.Boolean;

                    case (byte)'-':
                    case (byte)'0':
                    case (byte)'1':
                    case (byte)'2':
                    case (byte)'3':
                    case (byte)'4':
                    case (byte)'5':
                    case (byte)'6':
                    case (byte)'7':
                    case (byte)'8':
                    case (byte)'9':
                        return JsonValueType.Number;
                }
                throw new JsonParseException();
            }
        }

        public bool IsNull
        {
            get
            {
                var t = Buffer.Span;
                return t.SequenceEqual(NULL_MEMORY.Span);
            }
        }

        public bool GetBoolean()
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
                throw new JsonParseException();
            }
        }

        public Int32 GetInt32()
        {
            var t = Buffer.Span;
            if (Utf8Parser.TryParse(t, out Int32 value, out int _))
            {
                return value;
            }
            throw new JsonParseException();
        }

        public Single GetSingle()
        {
            var t = Buffer.Span;
            if (Utf8Parser.TryParse(t, out Single value, out int _))
            {
                return value;
            }
            throw new JsonParseException();
        }

        public Double GetDouble()
        {
            var t = Buffer.Span;
            if (Utf8Parser.TryParse(t, out Double value, out int _))
            {
                return value;
            }
            throw new JsonParseException();
        }
    }

    public class JsonParser
    {
        static bool IsSpace(byte b)
        {
            switch (b)
            {
                case (byte)' ':
                case (byte)'\r':
                case (byte)'\n':
                case (byte)'\t':
                    return true;
            }
            return false;
        }

        static bool IsNumber(byte b)
        {
            switch (b)
            {
                case (byte)'-':
                case (byte)'0':
                case (byte)'1':
                case (byte)'2':
                case (byte)'3':
                case (byte)'4':
                case (byte)'5':
                case (byte)'6':
                case (byte)'7':
                case (byte)'8':
                case (byte)'9':
                    return true;
            }
            return false;
        }

        public JsonSegment Parse(Memory<byte> src)
        {
            src = src.SkipWhile(b => IsSpace(b)).TakeWhile(b => !IsSpace(b));
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