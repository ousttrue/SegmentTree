using System;
using System.Linq;
using System.Text;


namespace SegmentTree.Json
{
    public static class JsonStringUnquote
    {
        public static int Unescape(string src, IStore w)
        {
            int writeCount = 0;
            Action<Char> Write = c =>
            {
                if (w != null)
                {
                    w.Write(c);
                }
                ++writeCount;
            };

            int i = 0;
            int length = src.Length - 1;
            while (i < length)
            {
                if (src[i] == '\\')
                {
                    var c = src[i + 1];
                    switch (c)
                    {
                        case '\\':
                        case '/':
                        case '"':
                            // remove prefix
                            Write(c);
                            i += 2;
                            continue;

                        case 'b':
                            Write('\b');
                            i += 2;
                            continue;
                        case 'f':
                            Write('\f');
                            i += 2;
                            continue;
                        case 'n':
                            Write('\n');
                            i += 2;
                            continue;
                        case 'r':
                            Write('\r');
                            i += 2;
                            continue;
                        case 't':
                            Write('\t');
                            i += 2;
                            continue;
                    }
                }

                Write(src[i]);
                i += 1;
            }
            while (i <= length)
            {
                Write(src[i++]);
            }

            return writeCount;
        }

        public static int Unescape(Utf8StringTmp s, IStore w)
        {
            int writeCount = 0;
            Action<Byte> Write = c =>
            {
                if (w != null)
                {
                    w.Write(c);
                }
                ++writeCount;
            };

            var it = s.GetIterator();
            while (it.MoveNext())
            {
                var l = it.CurrentByteLength;
                if (l == 1)
                {
                    if (it.Current == (Byte)'\\')
                    {
                        var c = it.Second;
                        switch (c)
                        {
                            case (Byte)'\\':
                            case (Byte)'/':
                            case (Byte)'"':
                                // remove prefix
                                Write(c);
                                it.MoveNext();
                                continue;

                            case (Byte)'b':
                                Write((Byte)'\b');
                                it.MoveNext();
                                continue;
                            case (Byte)'f':
                                Write((Byte)'\f');
                                it.MoveNext();
                                continue;
                            case (Byte)'n':
                                Write((Byte)'\n');
                                it.MoveNext();
                                continue;
                            case (Byte)'r':
                                Write((Byte)'\r');
                                it.MoveNext();
                                continue;
                            case (Byte)'t':
                                Write((Byte)'\t');
                                it.MoveNext();
                                continue;
                        }
                    }

                    Write(it.Current);
                }
                else if (l == 2)
                {
                    Write(it.Current);
                    Write(it.Second);
                }
                else if (l == 3)
                {
                    Write(it.Current);
                    Write(it.Second);
                    Write(it.Third);
                }
                else if (l == 4)
                {
                    Write(it.Current);
                    Write(it.Second);
                    Write(it.Third);
                    Write(it.Fourth);
                }
                else
                {
                    throw new ParseException("invalid utf8");
                }
            }

            return writeCount;
        }

        public static string Unescape(string src)
        {
            var sb = new StringBuilder();
            Unescape(src, new StringBuilderStore(sb));
            return sb.ToString();
        }

        public static int Unquote(string src, IStore w)
        {
            return Unescape(src.Substring(1, src.Length - 2), w);
        }

        public static int Unquote(Utf8StringTmp src, IStore w)
        {
            return Unescape(src.Subbytes(1, src.ByteLength - 2), w);
        }

        public static string Unquote(string src)
        {
            var count = Unquote(src, null);
            if (count == src.Length - 2)
            {
                return src.Substring(1, src.Length - 2);
            }
            else
            {
                var sb = new StringBuilder(count);
                Unquote(src, new StringBuilderStore(sb));
                var str = sb.ToString();
                return str;
            }
        }

        public static Utf8StringTmp Unquote(Utf8StringTmp src)
        {
            var count = Unquote(src, null);
            if (count == src.ByteLength - 2)
            {
                return src.Subbytes(1, src.ByteLength - 2);
            }
            else
            {
                var sb = new BytesStore(count);
                Unquote(src, sb);
                return new Utf8StringTmp(sb.Bytes);
            }
        }
    }
}
