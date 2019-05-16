using System;

namespace SegmentTree
{
    public static class SpanExtensions
    {
        public static Span<T> SkipWhile<T>(this Span<T> span, Func<T, bool> pred)
        {
            int i = 0;
            for (; i < span.Length; ++i)
            {
                if (pred(span[i]))
                {
                    continue;
                }
                break;
            }
            return span.Slice(i);
        }
        public static Span<T> TakeWhile<T>(this Span<T> span, Func<T, bool> pred)
        {
            int i = 0;
            for (; i < span.Length; ++i)
            {
                if (pred(span[i]))
                {
                    continue;
                }
                break;
            }
            return span.Slice(0, i);
        }
    }
}