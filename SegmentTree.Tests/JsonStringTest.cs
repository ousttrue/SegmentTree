using SegmentTree.Json;
using Xunit;

namespace SegmentTree.Tests
{
    public class JsonStringTest
    {
        [Fact]
        public void StringEscapeTest()
        {
            {
                var value = "\"";
                var escaped = "\\\"";
                Assert.Equal(escaped, JsonStringQuote.Escape(value));
                Assert.Equal(value, JsonStringUnquote.Unescape(escaped));
            }
            {
                var value = "\\";
                var escaped = "\\\\";
                Assert.Equal(escaped, JsonStringQuote.Escape(value));
                Assert.Equal(value, JsonStringUnquote.Unescape(escaped));
            }
            {
                var value = "/";
                var escaped = "\\/";
                Assert.Equal(escaped, JsonStringQuote.Escape(value));
                Assert.Equal(value, JsonStringUnquote.Unescape(escaped));
            }
            {
                var value = "\b";
                var escaped = "\\b";
                Assert.Equal(escaped, JsonStringQuote.Escape(value));
                Assert.Equal(value, JsonStringUnquote.Unescape(escaped));
            }
            {
                var value = "\f";
                var escaped = "\\f";
                Assert.Equal(escaped, JsonStringQuote.Escape(value));
                Assert.Equal(value, JsonStringUnquote.Unescape(escaped));
            }
            {
                var value = "\n";
                var escaped = "\\n";
                Assert.Equal(escaped, JsonStringQuote.Escape(value));
                Assert.Equal(value, JsonStringUnquote.Unescape(escaped));
            }
            {
                var value = "\r";
                var escaped = "\\r";
                Assert.Equal(escaped, JsonStringQuote.Escape(value));
                Assert.Equal(value, JsonStringUnquote.Unescape(escaped));
            }
            {
                var value = "\t";
                var escaped = "\\t";
                Assert.Equal(escaped, JsonStringQuote.Escape(value));
                Assert.Equal(value, JsonStringUnquote.Unescape(escaped));
            }
        }
    }
}