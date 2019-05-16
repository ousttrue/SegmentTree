using System;
using System.Buffers.Text;
using System.Runtime.InteropServices;
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
            m_childCount = 0;
            Buffer = buffer;
        }

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

                    case (byte)'"':
                        return JsonValueType.String;
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

        #region  Children
        int m_childCount;

        #region Array
        public int ArrayCount
        {
            get
            {
                return m_childCount;
            }
        }
        #endregion
        #endregion

        public string GetString()
        {
            var utf8 = new Utf8StringTmp(Buffer.ToArray());
            return JsonStringUnquote.Unquote(utf8).ToString();
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

        /**
         * null, true, false  or number
         */
        static bool IsToken(byte b)
        {
            if (IsSpace(b))
            {
                return false;
            }

            // delimiter
            switch (b)
            {
                case (byte)',':
                case (byte)']':
                case (byte)'}':
                    return false;
            }

            return true;
        }

        static bool GetStringToken(Utf8StringTmp src, int start, out int pos)
        {
            var target = (Byte)'"';

            var p = new Utf8Iterator(src.Bytes, start);
            while (p.MoveNext())
            {
                var b = p.Current;
                if (b <= 0x7F)
                {
                    // ascii
                    if (b == target/*'\"'*/)
                    {
                        // closed
                        pos = p.BytePosition;
                        return true;
                    }
                    else if (b == '\\')
                    {
                        // escaped
                        switch ((char)p.Second)
                        {
                            case '"': // fall through
                            case '\\': // fall through
                            case '/': // fall through
                            case 'b': // fall through
                            case 'f': // fall through
                            case 'n': // fall through
                            case 'r': // fall through
                            case 't': // fall through
                                      // skip next
                                p.MoveNext();
                                break;

                            case 'u': // unicode
                                      // skip next 4
                                p.MoveNext();
                                p.MoveNext();
                                p.MoveNext();
                                p.MoveNext();
                                break;

                            default:
                                // unkonw escape
                                throw new ParseException("unknown escape: " + p.Second);
                        }
                    }
                }
            }

            pos = -1;
            return false;
        }


        public static Memory<byte> GetToken(Memory<byte> src)
        {
            src = src.SkipWhile(b => IsSpace(b));

            var first = src.Span[0];
            switch (first)
            {
                case (byte)'[': // array
                    throw new NotImplementedException();

                case (byte)'{': // object
                    throw new NotImplementedException();

                case (byte)'"':
                    {
                        if (MemoryMarshal.TryGetArray(src, out ArraySegment<Byte> segment))
                        {
                            if (GetStringToken(new Utf8StringTmp(segment), 1, out int pos))
                            {
                                return src.Slice(0, pos + 1);
                            }
                            else
                            {
                                throw new ParseException("string end not found");
                            }
                        }
                        else
                        {
                            throw new ParseException("?");
                        }
                    }

                default:
                    return src.TakeWhile(b => IsToken(b));
            }
        }

        public JsonSegment Parse(Memory<byte> src)
        {
            return new JsonSegment(GetToken(src));
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