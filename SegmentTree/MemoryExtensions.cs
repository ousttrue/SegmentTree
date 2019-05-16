using System;

namespace SegmentTree
{
    public static class MemoryExtensions
    {
        public static Memory<T> SkipWhile<T>(this Memory<T> src, Func<T, bool> pred)
        {
            int i = 0;
            var span = src.Span;
            for (; i < src.Length; ++i)
            {
                if (pred(span[i]))
                {
                    continue;
                }
                break;
            }
            return src.Slice(i);
        }
        public static Memory<T> TakeWhile<T>(this Memory<T> src, Func<T, bool> pred)
        {
            int i = 0;
            var span = src.Span;
            for (; i < src.Length; ++i)
            {
                if (pred(span[i]))
                {
                    continue;
                }
                break;
            }
            return src.Slice(0, i);
        }
    }
}