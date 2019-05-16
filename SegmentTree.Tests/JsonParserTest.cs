using System;
using SegmentTree.Json;
using Xunit;

namespace SegmentTree.Tests
{
    public class JsonParserTest
    {
        [Fact]
        public void TestNull()
        {
            var p = new JsonParser();
            var parsed = p.Parse("null");
            Assert.True(parsed.IsNull);
        }

        [Fact]
        public void TestBool()
        {
            var p = new JsonParser();
            {
                var parsed = p.Parse("true");
                Assert.True(parsed.GetBool());
            }

            {
                var parsed = p.Parse("false");
                Assert.False(parsed.GetBool());
            }
        }
    }
}
