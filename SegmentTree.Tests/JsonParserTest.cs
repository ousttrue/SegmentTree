using System;
using System.Linq;
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
        public void TestTokenize()
        {
            {
                var encoding = new UTF8Encoding(false);
                var quoted = " \"fuga\\n  hoge\" ";
                var src = encoding.GetBytes(quoted);
                var token = JsonParser.Tokenize(new ArraySegment<byte>(src)).First();
                Assert.Equal("\"fuga\\n  hoge\"", encoding.GetString(token.ToArraySegment(src)));
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

        [Fact]
        public void TestArray()
        {
            var p = new JsonParser();
            {
                var json = "[]";
                var parsed = p.Parse(json);
                Assert.Equal(JsonValueType.Array, parsed.ValueType);
                Assert.Equal(0, parsed.ArrayCount);
            }

            {
                var json = "[1,2,3]";
                var parsed = p.Parse(json);
                Assert.Equal(JsonValueType.Array, parsed.ValueType);
                Assert.Equal(1, parsed[0].GetInt32());
                Assert.Equal(2, parsed[1].GetInt32());
                Assert.Equal(3, parsed[2].GetInt32());
            }

            {
                var json = "[\"key\",1]";
                var parsed = p.Parse(json);

                Assert.Equal(JsonValueType.Array, parsed.ValueType);

                var it = parsed.AsArray.GetEnumerator();

                Assert.True(it.MoveNext());
                Assert.Equal("key", it.Current.GetString());

                Assert.True(it.MoveNext());
                Assert.Equal(1, it.Current.GetDouble());

                Assert.False(it.MoveNext());

                Assert.Equal("key", parsed[0].GetString());
                Assert.Equal(1, parsed[1].GetDouble());

                //Assert.Equal("[\n  \"key\",\n  1\n]", parsed.ToString("  "));
            }
        }

        [Fact]
        public void TestObject()
        {
            var p = new JsonParser();
            {
                var json = "{}";
                var parsed = p.Parse(json);
                Assert.Equal(JsonValueType.Object, parsed.ValueType);
                Assert.Equal(0, parsed.AsObject.Count());
            }

            {
                var json = "{\"key\":\"value\"}";
                var parsed = p.Parse(json);
                Assert.Equal(JsonValueType.Object, parsed.ValueType);

                var it = parsed.AsObject.GetEnumerator();

                Assert.True(it.MoveNext());
                Assert.Equal("key", it.Current.Key.GetString());
                Assert.Equal("value", it.Current.Value.GetString());

                Assert.False(it.MoveNext());
            }
        }

        [Fact]
        public void TestNestedObject()
        {
            var p = new JsonParser();
            {
                var json = "{\"key\":{ \"nestedKey\": \"nestedValue\" }, \"key2\": { \"nestedKey2\": \"nestedValue2\" } }";
                var parsed = p.Parse(json);
                Assert.Equal(JsonValueType.Object, parsed.ValueType);

                {
                    var it = parsed.AsObject.GetEnumerator();

                    Assert.True(it.MoveNext());
                    Assert.Equal("key", it.Current.Key.GetString());
                    Assert.Equal(JsonValueType.Object, it.Current.Value.ValueType);

                    Assert.True(it.MoveNext());
                    Assert.Equal("key2", it.Current.Key.GetString());
                    Assert.Equal(JsonValueType.Object, it.Current.Value.ValueType);

                    Assert.False(it.MoveNext());
                }

                var nested = parsed["key2"];

                {
                    var it = nested.AsObject.GetEnumerator();

                    Assert.True(it.MoveNext());
                    Assert.Equal("nestedKey2", it.Current.Key.GetString());
                    Assert.Equal("nestedValue2", it.Current.Value.GetString());

                    Assert.False(it.MoveNext());
                }

                Assert.Equal("nestedValue2", parsed["key2"]["nestedKey2"].GetString());
            }
        }
    }
}
