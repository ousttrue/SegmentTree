using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;

namespace SegmentTree.Json
{
    public enum JsonValueType
    {
        Null,
        Boolean,

        Number,

        String,

        Array,

        Object,
    }

    public struct JsonNode
    {
        Byte[] m_bytes;

        List<JsonSegment> m_segments;

        int m_index;

        JsonSegment Segment
        {
            get
            {
                return m_segments[m_index];
            }
        }

        Memory<byte> Buffer
        {
            get
            {
                var seg = Segment;
                return new Memory<byte>(m_bytes, seg.Offset, seg.Count);
            }
        }

        static Memory<byte> MemoryFromStr(string src)

        {
            return Encoding.UTF8.GetBytes(src);
        }
        public static readonly Memory<byte> NULL_MEMORY = MemoryFromStr("null");
        public static readonly Memory<byte> TRUE_MEMORY = MemoryFromStr("true");
        public static readonly Memory<byte> FALSE_MEMORY = MemoryFromStr("false");

        /**
        * https://www.json.org/
        */
        public JsonValueType ValueType
        {
            get
            {
                var ch = Buffer.Span[0];
                switch (ch)
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
                        return JsonValueType.Number;

                    case (byte)'n':
                        return JsonValueType.Null;

                    case (byte)'t':
                    case (byte)'f': // fall through
                        return JsonValueType.Boolean;

                    case (byte)'"':
                        return JsonValueType.String;

                    case (byte)'[':
                        return JsonValueType.Array;
                    case (byte)'{':
                        return JsonValueType.Object;
                }
                throw new ParseException();
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
                throw new ParseException();
            }
        }

        public Int32 GetInt32()
        {
            var t = Buffer.Span;
            if (Utf8Parser.TryParse(t, out Int32 value, out int _))
            {
                return value;
            }
            throw new ParseException();
        }

        public Single GetSingle()
        {
            var t = Buffer.Span;
            if (Utf8Parser.TryParse(t, out Single value, out int _))
            {
                return value;
            }
            throw new ParseException();
        }

        public Double GetDouble()
        {
            var t = Buffer.Span;
            if (Utf8Parser.TryParse(t, out Double value, out int _))
            {
                return value;
            }
            throw new ParseException();
        }

        public string GetString()
        {
            var utf8 = new Utf8StringTmp(Buffer.ToArray());
            return JsonStringUnquote.Unquote(utf8).ToString();
        }

        public int ArrayCount
        {
            get
            {
                if (ValueType != JsonValueType.Array)
                {
                    throw new InvalidOperationException("is not array");
                }
                return Segment.ChildCount;
            }
        }

        public int ObjectCount
        {
            get
            {
                if (ValueType != JsonValueType.Object)
                {
                    throw new InvalidOperationException("is not object");
                }
                return Segment.ChildCount / 2;
            }
        }

        public JsonNode(Byte[] bytes, List<JsonSegment> segments, int index)
        {
            m_bytes = bytes;
            m_segments = segments;
            m_index = index;
        }

        public override string ToString()
        {
            var seg = Segment;
            var utf8 = new Utf8StringTmp(m_bytes, seg.Offset, seg.Count);
            return $"<{utf8}>";
        }
    }
}