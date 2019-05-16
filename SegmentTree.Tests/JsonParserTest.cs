using System;
using System.Text;
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
            Assert.Equal(JsonValueType.Null, parsed.ValueType);
            Assert.True(parsed.IsNull);
        }

        [Fact]
        public void TestBool()
        {
            var p = new JsonParser();
            {
                var parsed = p.Parse("true");
                Assert.Equal(JsonValueType.Boolean, parsed.ValueType);
                Assert.True(parsed.GetBoolean());
            }

            {
                var parsed = p.Parse("false");
                Assert.Equal(JsonValueType.Boolean, parsed.ValueType);
                Assert.False(parsed.GetBoolean());
            }
        }

        [Fact]
        public void TestNumber()
        {
            var p = new JsonParser();
            {
                var parsed = p.Parse("1");
                Assert.Equal(JsonValueType.Number, parsed.ValueType);
                Assert.Equal(1, parsed.GetInt32());
                Assert.ThrowsAny<ParseException>(() => parsed.GetBoolean());
            }
            {
                var parsed = p.Parse(" 22 ");
                Assert.Equal(JsonValueType.Number, parsed.ValueType);
                Assert.Equal(22, parsed.GetInt32());
            }
            {
                var parsed = p.Parse(" 3.3 ");
                Assert.Equal(JsonValueType.Number, parsed.ValueType);
                Assert.Equal(3, parsed.GetInt32());
                Assert.Equal(3.3f, parsed.GetSingle());
            }
            {
                var parsed = p.Parse(" -4.44444444444444444444 ");
                Assert.Equal(JsonValueType.Number, parsed.ValueType);
                Assert.Equal(-4, parsed.GetInt32());
                Assert.Equal(-4.44444444444444444444, parsed.GetDouble());
            }
            {
                var parsed = p.Parse(" -5e-4 ");
                Assert.Equal(JsonValueType.Number, parsed.ValueType);
                Assert.Equal(-5e-4, parsed.GetDouble());
                Assert.Equal(-5, parsed.GetInt32());
            }
        }

        [Fact]
        public void TestString()
        {
            var p = new JsonParser();
            {
                var value = "hoge";
                var quoted = "\"hoge\"";
                Assert.Equal(quoted, JsonStringQuote.Quote(value));
                var parsed = p.Parse(quoted);
                Assert.Equal(JsonValueType.String, parsed.ValueType);
                Assert.Equal("hoge", parsed.GetString());
            }

            {
                var value = "fuga\n  hoge";
                var quoted = "\"fuga\\n  hoge\"";
                Assert.Equal(quoted, JsonStringQuote.Quote(value));
                var parsed = p.Parse(quoted);
                Assert.Equal(JsonValueType.String, parsed.ValueType);
                Assert.Equal(value, parsed.GetString());
            }
        }
    }
}
