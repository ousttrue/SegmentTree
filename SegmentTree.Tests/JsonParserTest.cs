using System;
using SegmentTree.Json;
using Xunit;

namespace SegmentTree.Tests
{
    public class JsonParserTest
    {
        [Fact]
        public void Test1()
        {
            var p = new JsonParser();
            var parsed = p.Parse("null");
            Assert.True(parsed.IsNull);
        }
    }
}
