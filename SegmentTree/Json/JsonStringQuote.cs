using System;
using System.Linq;
using System.Text;


namespace SegmentTree
{
    public static class JsonStringQuote
    {
        public static void Escape(String s, IStore w)
        {
            if (String.IsNullOrEmpty(s))
            {
                return;
            }

            var it = s.ToCharArray().Cast<char>().GetEnumerator();
            while (it.MoveNext())
            {
                switch (it.Current)
                {
                    case '"':
                    case '\\':
                    case '/':
                        // \\ prefix
                        w.Write('\\');
                        w.Write(it.Current);
                        break;

                    case '\b':
                        w.Write('\\');
                        w.Write('b');
                        break;
                    case '\f':
                        w.Write('\\');
                        w.Write('f');
                        break;
                    case '\n':
                        w.Write('\\');
                        w.Write('n');
                        break;
                    case '\r':
                        w.Write('\\');
                        w.Write('r');
                        break;
                    case '\t':
                        w.Write('\\');
                        w.Write('t');
                        break;

                    default:
                        w.Write(it.Current);
                        break;
                }
            }
        }

        public static void Escape(Utf8StringTmp s, IStore w)
        {
            if (s.IsEmpty)
            {
                return;
            }

            var it = s.GetIterator();
            while (it.MoveNext())
            {
                var l = it.CurrentByteLength;
                if (l == 1)
                {
                    var b = it.Current;
                    switch (b)
                    {
                        case (Byte)'"':
                        case (Byte)'\\':
                        case (Byte)'/':
                            // \\ prefix
                            w.Write((Byte)'\\');
                            w.Write(b);
                            break;

                        case (Byte)'\b':
                            w.Write((Byte)'\\');
                            w.Write((Byte)'b');
                            break;
                        case (Byte)'\f':
                            w.Write((Byte)'\\');
                            w.Write((Byte)'f');
                            break;
                        case (Byte)'\n':
                            w.Write((Byte)'\\');
                            w.Write((Byte)'n');
                            break;
                        case (Byte)'\r':
                            w.Write((Byte)'\\');
                            w.Write((Byte)'r');
                            break;
                        case (Byte)'\t':
                            w.Write((Byte)'\\');
                            w.Write((Byte)'t');
                            break;

                        default:
                            w.Write(b);
                            break;
                    }
                    // ascii
                }
                else if (l == 2)
                {
                    w.Write(it.Current);
                    w.Write(it.Second);
                }
                else if (l == 3)
                {
                    w.Write(it.Current);
                    w.Write(it.Second);
                    w.Write(it.Third);
                }
                else if (l == 4)
                {
                    w.Write(it.Current);
                    w.Write(it.Second);
                    w.Write(it.Third);
                    w.Write(it.Fourth);
                }
                else
                {
                    throw new ParseException("invalid utf8");
                }
            }
        }

        public static string Escape(String s)
        {
            var sb = new StringBuilder();
            Escape(s, new StringBuilderStore(sb));
            return sb.ToString();
        }

        public static void Quote(String s, IStore w)
        {
            w.Write('"');
            Escape(s, w);
            w.Write('"');
        }

        public static void Quote(Utf8StringTmp s, IStore w)
        {
            w.Write((Byte)'"');
            Escape(s, w);
            w.Write((Byte)'"');
        }

        /// <summary>
        /// Added " and Escape
        /// </summary>
        /// <param name="s"></param>
        /// <param name="w"></param>
        public static string Quote(string s)
        {
            var sb = new StringBuilder();
            Quote(s, new StringBuilderStore(sb));
            return sb.ToString();
        }

        public static Utf8StringTmp Quote(Utf8StringTmp s)
        {
            var sb = new BytesStore(s.ByteLength);
            Quote(s, sb);
            return new Utf8StringTmp(sb.Bytes);
        }
    }
}
