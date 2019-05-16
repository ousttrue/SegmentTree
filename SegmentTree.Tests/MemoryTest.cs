using System;
using System.Text;
using Xunit;

namespace SegmentTree.Tests
{
    public class MemoryTest
    {
        [Fact]
        public void TestSkip()
        {
            {
                var lhs = Encoding.ASCII.GetBytes("X").AsSpan();
                var rhs = Encoding.ASCII.GetBytes("  X").AsMemory().SkipWhile(b => b == ' ').Span;
                Assert.True(lhs.SequenceEqual(rhs));
            }
            {
                var lhs = Encoding.ASCII.GetBytes("XXX").AsSpan();
                var rhs = Encoding.ASCII.GetBytes("XXX").AsMemory().SkipWhile(b => b == ' ').Span;
                Assert.True(lhs.SequenceEqual(rhs));
            }
        }

        [Fact]
        public void TestTake()
        {
            {
                var lhs = Encoding.ASCII.GetBytes("X").AsSpan();
                var rhs = Encoding.ASCII.GetBytes("X  ").AsMemory().TakeWhile(b => b == 'X').Span;
                Assert.True(lhs.SequenceEqual(rhs));
            }
            {
                var lhs = Encoding.ASCII.GetBytes("XXX").AsSpan();
                var rhs = Encoding.ASCII.GetBytes("XXX").AsMemory().TakeWhile(b => b == 'X').Span;
                Assert.True(lhs.SequenceEqual(rhs));
            }
        }
    }
}