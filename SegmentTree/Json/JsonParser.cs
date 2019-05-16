using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SegmentTree.Json
{

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

        static int FindStringEnd(Byte[] array, int start)
        {
            var target = (Byte)'"';

            var p = new Utf8Iterator(array, start + 1);
            while (p.MoveNext())
            {
                var b = p.Current;
                if (b <= 0x7F)
                {
                    // ascii
                    if (b == target/*'\"'*/)
                    {
                        // closed
                        return p.BytePosition;
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
                                // unknown escape
                                throw new ParseException("unknown escape: " + p.Second);
                        }
                    }
                }
            }

            return -1;
        }

        public struct Token
        {
            public readonly int Offset;
            public readonly int Count;

            public Token(int offset, int count)
            {
                Offset = offset;
                Count = count;
            }

            public ArraySegment<byte> ToArraySegment(byte[] array)
            {
                return new ArraySegment<byte>(array, Offset, Count);
            }
        }

        public static IEnumerable<Token> Tokenize(ArraySegment<byte> src)
        {
            var count = src.Offset + src.Count;
            int j = src.Offset;
            while (true)
            {
                // skip white space
                for (; j < count; ++j)
                {
                    if (!IsSpace(src.Array[j]))
                    {
                        break;
                    }
                }
                if (j >= count)
                {
                    yield break;
                }

                var first = src.Array[j];
                switch (first)
                {
                    case (byte)'[': // array
                    case (byte)']': // array
                    case (byte)'{': // object
                    case (byte)'}': // object
                    case (byte)':': // object
                    case (byte)',': // array, object
                        {
                            yield return new Token(j, 1);
                            ++j;
                        }
                        break;

                    case (byte)'"':
                        {
                            var end = FindStringEnd(src.Array, j);
                            if (end >= 0)
                            {
                                var length = end - j + 1;
                                yield return new Token(j, length);
                                j += length;
                            }
                            else
                            {
                                throw new ParseException("string end not found");
                            }
                        }
                        break;

                    default:
                        {
                            var length = 0;
                            for (; j + length < count; ++length)
                            {
                                if (!IsToken(src.Array[j + length]))
                                {
                                    break;
                                }
                            }
                            yield return new Token(j, length);
                            j += length;
                        }
                        break;
                }
            }
        }

        Stack<int> m_current = new Stack<int>();
        List<JsonSegment> m_segments = new List<JsonSegment>();

        public JsonNode Parse(ArraySegment<byte> src)
        {
            m_segments.Clear();

            m_current.Clear();
            m_current.Push(-1); // dummy

            foreach (var token in Tokenize(src))
            {
                var head = src.Array[token.Offset];
                if (head == '[' || head == '{')
                {
                    var parentIndex = m_current.Peek();
                    m_segments.Add(new JsonSegment(parentIndex, token.Offset, token.Count));
                    if (parentIndex >= 0)
                    {
                        m_segments[parentIndex] = m_segments[parentIndex].IncrementChildCount();
                    }
                    m_current.Push(m_segments.Count - 1);
                }
                else if (head == ']' || head == '}')
                {
                    if (m_current.Count <= 1)
                    {
                        throw new ParseException("too many close");
                    }
                    var parentIndex = m_current.Peek();
                    m_segments[parentIndex] = m_segments[parentIndex].ExtendTo(token.Offset + 1);
                    m_current.Pop();
                }
                else
                {
                    var parentIndex = m_current.Peek();
                    m_segments.Add(new JsonSegment(parentIndex, token.Offset, token.Count));
                    if (parentIndex >= 0)
                    {
                        m_segments[parentIndex] = m_segments[parentIndex].IncrementChildCount();
                    }
                }
            }

            if (m_current.Count != 1)
            {
                throw new ParseException("array or object not closed");
            }

            return new JsonNode(src.Array, m_segments, 0);
        }
    }

    public static class JsonParserExtensions
    {
        public static JsonNode Parse(this JsonParser p, string src)
        {
            var bytes = Encoding.UTF8.GetBytes(src);
            return p.Parse(bytes);
        }
    }
}