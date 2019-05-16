using System;
using System.Buffers.Text;
using System.Text;

namespace SegmentTree.Json
{
    public struct JsonSegment
    {
        public readonly int Offset;

        public readonly int Count;

        public readonly int ParentIndex;

        public readonly int ChildCount;

        public JsonSegment(int parentIndex, int offset, int length, int childCount = 0)
        {
            ParentIndex = parentIndex;
            Offset = offset;
            Count = length;
            ChildCount = childCount;
        }

        public JsonSegment IncrementChildCount()
        {
            return new JsonSegment(ParentIndex, Offset, Count, ChildCount + 1);
        }

        public JsonSegment ExtendTo(int offset)
        {
            return new JsonSegment(ParentIndex, Offset, offset - Offset, ChildCount);
        }
    }
}