using System;
using Xunit;

namespace HttpPreferHeaderParser.Test
{
    public class PreferHeaderParserTest
    {
       
        [Theory]
        [InlineData("foo", 1)]
        [InlineData("foo,bar", 2)]
        [InlineData("foo=42", 1)]
        [InlineData("foo=\"bar, baz; qux\"", 1)]
        [InlineData("foo; bar", 1)]
        [InlineData("foo=\"\"; bar", 1)]
        [InlineData("foo; bar=\"\"", 1)]
        [InlineData("respond-async, wait=100, handling=lenient", 3)]

        public void Test001_PreferenceCount(string headerValue, int numberOfPreferences)
        {
            var objectUnderTest = new PreferHeaderParser();

            var preferences = objectUnderTest.Parse(headerValue);

            Assert.NotNull(preferences);
            Assert.Equal(numberOfPreferences, preferences.Count);

        }

        [Fact]
        public void Test002_MultiPreferenceValueCheck()
        {
            var objectUnderTest = new PreferHeaderParser();
            var preferences = objectUnderTest.Parse("respond-async, wait=100, handling=lenient");

            Assert.NotNull(preferences);
            Assert.Equal(3, preferences.Count);

            Assert.Equal("respond-async", preferences[0].Name);
            Assert.Equal("true", preferences[0].Value);
            Assert.Empty(preferences[0].Parameters);

            Assert.Equal("wait", preferences[1].Name);
            Assert.Equal("100", preferences[1].Value);
            Assert.Empty(preferences[1].Parameters);

            Assert.Equal("handling", preferences[2].Name);
            Assert.Equal("lenient", preferences[2].Value);
            Assert.Empty(preferences[2].Parameters);
        }

        [Theory]
        [InlineData("foo; bar")]
        [InlineData("foo=\"\"; bar")]
        [InlineData("foo; bar=\"\"")]
        public void Test003_MultiPreferenceParameterExpression(string headerValue)
        {
            var objectUnderTest = new PreferHeaderParser();
            var preferences = objectUnderTest.Parse(headerValue);

            Assert.NotNull(preferences);
            Assert.Single(preferences);

            Assert.Equal("foo", preferences[0].Name);
            Assert.Equal("true", preferences[0].Value);
            Assert.Single(preferences[0].Parameters);

            Assert.True(preferences[0].Parameters.ContainsKey("bar"));
            Assert.Equal("true", preferences[0].Parameters["bar"]);
        }
        

        [Fact]
        public void Test004_IgnoreDuplicatePreferenceValues()
        {
            var objectUnderTest = new PreferHeaderParser();
            var preferences = objectUnderTest.Parse("wait = 100, wait = 200");

            Assert.NotNull(preferences);
            Assert.Single(preferences);

        }

        [Fact]
        public void Test005_ParseListOfHeaders()
        {
            var objectUnderTest = new PreferHeaderParser();

            var preferences = objectUnderTest.Parse(new string[] { "respond-async", "wait=100", "handling=lenient" });
            Assert.NotNull(preferences);
            Assert.Equal(3, preferences.Count);
        }

        [Fact]
        public void Test006_HandleComplicatedPreferences()
        {
            var objectUnderTest = new PreferHeaderParser();
            var preferences = objectUnderTest.Parse(new string[]
            {
                "respond-async, wait = 100",
                "handling=\"strict\"; abort-early; path-format = \"json-pointer\"",
                "response-cache-headers= \"etag, last-modified\"",
                "sure lets have some spaces = \"where's waldo?\"; ok=200",
                "wait=400"
            });

            Assert.NotNull(preferences);
            Assert.Equal(5, preferences.Count);

            /* Check Values */
            Assert.Equal("respond-async", preferences[0].Name);
            Assert.Equal("true", preferences[0].Value);
            Assert.Empty(preferences[0].Parameters);

            Assert.Equal("wait", preferences[1].Name);
            Assert.Equal("100", preferences[1].Value);
            Assert.Empty(preferences[1].Parameters);

            Assert.Equal("handling", preferences[2].Name);
            Assert.Equal("strict", preferences[2].Value);            
            Assert.NotEmpty(preferences[2].Parameters);
            Assert.Equal(2, preferences[2].Parameters.Count);
            Assert.True(preferences[2].Parameters.ContainsKey("abort-early"));
            Assert.Equal("true", preferences[2].Parameters["abort-early"]);
            Assert.True(preferences[2].Parameters.ContainsKey("path-format"));
            Assert.Equal("json-pointer", preferences[2].Parameters["path-format"]);

            Assert.Equal("response-cache-headers", preferences[3].Name);
            Assert.Equal("etag, last-modified", preferences[3].Value);

            Assert.Equal("sureletshavesomespaces", preferences[4].Name);
            Assert.Equal("where's waldo?", preferences[4].Value);
            Assert.Single(preferences[4].Parameters);
            Assert.True(preferences[4].Parameters.ContainsKey("ok"));
            Assert.Equal("200", preferences[4].Parameters["ok"]);
        }

        [Fact]
        public void Test007_MultiPartPreference()
        {
            var objectUnderTest = new PreferHeaderParser();
            var preferences = objectUnderTest.Parse("left-part = 47; right-part = 56");

            Assert.NotNull(preferences);
            Assert.Single(preferences);
            Assert.Equal("left-part", preferences[0].Name);
            Assert.Equal("47", preferences[0].Value);
            Assert.Single(preferences[0].Parameters);
            Assert.True(preferences[0].Parameters.ContainsKey("right-part"));
            Assert.Equal("56", preferences[0].Parameters["right-part"]);
        }

        [Fact]
        public void Test008_EmptyHeader()
        {
            var objectUnderTest = new PreferHeaderParser();
            var preferences = objectUnderTest.Parse("");

            Assert.Null(preferences);            
        }

        [Fact]
        public void Test009_EmptyArray()
        {
            var objectUnderTest = new PreferHeaderParser();
            var preferences = objectUnderTest.Parse(Array.Empty<string>());

            Assert.Null(preferences);
        }

        [Fact]
        public void Test010_SendNullString()
        {
            var objectUnderTest = new PreferHeaderParser();
            string nullString = null;
            var preferences = objectUnderTest.Parse(nullString);

            Assert.Null(preferences);
        }

        [Fact]
        public void Test011_SendNullStringArray()
        {
            var objectUnderTest = new PreferHeaderParser();
            string[] nullStringArray = null;
            var preferences = objectUnderTest.Parse(nullStringArray);

            Assert.Null(preferences);
        }
    }
}
